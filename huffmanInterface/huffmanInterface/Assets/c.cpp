QString Controller::renderTemplate(const QString &templateName, const QVariantMap &keys) {
  QString rendered = Controller::templates.value(templateName);
  while (Controller::replaceReg.indexIn(rendered, 0) != -1) {
    const QString expr = Controller::replaceReg.cap(0);
    const QString key = Controller::replaceReg.cap(1);
    const QString type = Controller::replaceReg.cap(2);
    const QString templ = Controller::replaceReg.cap(3);
    const QString alternative = Controller::replaceReg.cap(4);

    if (!keys.contains(key)) {
      rendered.replace(expr, alternative);
      continue;
    }

    QString after = "";
    QVariant value = keys.value(key);

    if (type.length() == 2) {
      if (value.type() != QVariant::List) {
        qDebug() << "Type mismatch " << value.type() << ", List is requered.";
      } else {
        const QVariantList list = value.toList();
        for (QVariantList::ConstIterator item = list.constBegin();
             item != list.constEnd(); ++item) {
          if (type == "%%") {
            if (item->type() != QVariant::String && item->type() != QVariant::Int && item->type() != QVariant::Double) {
              qDebug() << "Type mismatch " << item->type() << "; string, int or double is requered.";
            } else {
              after += QString(templ).replace("$", item->toString());
            }
          } else {
            if (item->type() != QVariant::Map) {
              qDebug() << "Type mismatch " << item->type() << "; string, int or double is requered.";
            } else {
              after += Controller::renderTemplate(templ, item->toMap());
            }
          }
        }
      }
    } else if (type == "#") {
      if (value.type() == QVariant::Map) {
        after = Controller::renderTemplate(templ, value.toMap());
      } else {
        qDebug() << "Type mismatch " << value.type() << "; dict is requered.";
      }
    } else {
      if (value.type() == QVariant::String ||
          value.type() == QVariant::Int ||
          value.type() == QVariant::Double) {
        after = QString(templ).replace("$", value.toString());
      } else {
        qDebug() << "Type mismatch " << value.type() << "; string, int or double is requered.";
      }
    }

    rendered.replace(expr, after);
  }
  return rendered;
}

QString Controller::getGraph(const QList<int> &real, const QList<double> &approx) {
  QFile csv(Controller::workingPath + "tmp/approx.csv");
  csv.open(QIODevice::WriteOnly);
  QStringList realStr, approxStr;
  for (QList<int>::ConstIterator it = real.constBegin(); it != real.constEnd(); ++it) {
    realStr.push_back(QString::number(*it));
  }

  for (QList<double>::ConstIterator it = approx.constBegin(); it != approx.constEnd(); ++it) {
    approxStr.push_back(QString::number(*it));
  }

  csv.write(QString(realStr.join("; ") + "\n").toUtf8());
  csv.write(QString(approxStr.join("; ") + "\n").toUtf8());
  csv.close();

  QProcess worker;
  worker.setWorkingDirectory(Controller::workingPath + "tmp/");
  worker.start("sh", QStringList() << Controller::workingPath + "tmp/start.sh"
               << "1" << "1"
               << Controller::workingPath + "tmp/graph.r"
               << Controller::workingPath + "tmp/graph.log");

  worker.waitForFinished(1000);
  csv.remove();
  if (worker.exitStatus() == QProcess::NormalExit) {
    return Controller::workingPath + "tmp/graph.jpg";
  } else {
    return Controller::workingPath + "imgs/non-graph.jpg";
  }
}

QString Controller::getConstPage(const QString &name) {
  return Controller::renderTemplate(name, QVariantMap());
}

// Slots
void Controller::search(const QString &names_, const QString &type) {
  if (this->locked) {
    qDebug() << "Controller already has task!";
    return;
  } else {
    this->locked = true;
  }


  this->cleanUp();
  const QStringList engines = Controller::handlers.value(type);
  this->searchHandler = type;
  QStringList names = names_.split(",");

  this->timeout = names.size() * 1000 * 60;
  emit this->setTimeOut(this->timeout);

  for (QStringList::Iterator name = names.begin(); name != names.end(); name++) {
    (*name) = name->trimmed();
  }
  for (QStringList::ConstIterator engine = engines.constBegin();
       engine != engines.constEnd(); ++engine) {
    this->searchBy(names, *engine);
  }
  emit this->setInfo("Starting search.");
}

void Controller::searchBy(const QStringList &names, const QString &engine) {
  for (QStringList::ConstIterator name = names.constBegin();
       name != names.constEnd(); name++) {
    const SearchTask task = SearchTask(*name, engine);
    this->sheduleTask(task);
  }
}

void Controller::sheduleTask(const SearchTask &task) {
  ScriptRunner* scriptRunner = new ScriptRunner(task, Controller::workingPath + "tmp/",
                                                Controller::commands.value(task.second),
                                                this->timeout);
  QThread* thread = new QThread();
  scriptRunner->moveToThread(thread);

  connect(thread, SIGNAL(started()), scriptRunner, SLOT(search()), Qt::QueuedConnection);

  connect(thread, SIGNAL(finished()), thread, SLOT(deleteLater()), Qt::QueuedConnection);
  connect(thread, SIGNAL(finished()), scriptRunner, SLOT(deleteLater()), Qt::QueuedConnection);

  connect(scriptRunner, SIGNAL(complite(SearchTask,QVariantMap)),
          this, SLOT(addResult(SearchTask,QVariantMap)), Qt::QueuedConnection);
  connect(scriptRunner, SIGNAL(complite(SearchTask,QVariantMap)),
          thread, SLOT(quit()), Qt::QueuedConnection);

  this->workers.insert(task, thread);
  this->tasks += 1;
  this->tasksMax += 1;

  thread->start(QThread::NormalPriority);
}

void Controller::addResult(const SearchTask &task, const QVariantMap &keys) {
  const QString name = task.first;
  const QString engine = task.second;
  if (!this->incompliteKeys.contains(name)) {
    this->incompliteKeys[name] = QVariant(QVariantMap());
  }

  QVariantMap newResults = this->incompliteKeys[name].toMap();
  newResults[engine] = keys;
  this->incompliteKeys[name] = QVariant(newResults);
  this->tasks--;
  emit this->setInfo("Complite " + QString::number(this->tasksMax - this->tasks) + "/" + QString::number(this->tasksMax) + " tasks.");
  if (this->tasks <= 0) {
    this->processResults();
    this->cleanUp();
    this->locked = false;
  }
}

void Controller::cleanUp() {
  this->tasks = 0;
  this->tasksMax = 0;
  this->workers.clear();
  //this->incompliteKeys.clear();
  this->searchHandler.clear();
}

QVariantList Controller::mapToList(const QString &key, const QVariantMap &map) {
  QVariantList result;
  for (QVariantMap::ConstIterator it = map.constBegin();
       it != map.constEnd(); ++it) {
    QVariantMap item = it->toMap();
    item.insert(key, it.key());
    result.push_back(QVariant(item));
  }
  return result;
}

void Controller::processResults() {
  const bool multiEngine = !(Controller::handlers.value(this->searchHandler).length() == 1);
  const bool multiPerson = !(this->incompliteKeys.size() == 1);

  QString title = "Results";

  QVariantMap result;
  result.insert("service", QVariant(this->searchHandler));

  if (multiPerson) {
    QVariantList persons;
    for (QVariantMap::ConstIterator personIt = this->incompliteKeys.constBegin();
         personIt != this->incompliteKeys.constEnd(); ++personIt) {
      QVariantMap person = personIt.value().toMap();
      QString personName = personIt.key();
      if (multiEngine) {
        QVariantList engineResults;
        for (QVariantMap::ConstIterator engineRes = person.constBegin();
             engineRes != person.constEnd(); ++engineRes) {
          QString engineName = engineRes.key();
          QVariantMap singleResult = engineRes.value().toMap();
          singleResult.insert("engine", QVariant(Controller::engineRepr[engineName]));
          engineResults.push_back(singleResult);
        }
        QVariantMap newPerson;
        newPerson.insert("name", QVariant(personName));
        newPerson.insert("results", QVariant(engineResults));
        persons.push_back(QVariant(newPerson));
      } else {
        QString engineName = person.keys().first();
        QVariantMap singleResult = person[engineName].toMap();
        singleResult.insert("engine", QVariant(Controller::engineRepr[engineName]));
        singleResult.insert("name", QVariant(personName));
        persons.push_back(QVariant(singleResult));
      }
    }
    result.insert("persons", persons);
  } else {
    QString personName = this->incompliteKeys.keys().first();
    title = personName;

    QVariantMap person = this->incompliteKeys[personName].toMap();
    if (multiEngine) {
      QVariantList engineResults;
      for (QVariantMap::ConstIterator engineRes = person.constBegin();
           engineRes != person.constEnd(); ++engineRes) {
        QString engineName = engineRes.key();
        QVariantMap singleResult = engineRes.value().toMap();
        singleResult.insert("engine", QVariant(Controller::engineRepr[engineName]));
        engineResults.push_back(singleResult);
      }
      QVariantMap newPerson;
      newPerson.insert("name", QVariant(personName));
      newPerson.insert("results", QVariant(engineResults));
      person = newPerson;
    } else {
      QString engineName = person.keys().first();
      QVariantMap engineResult = person[engineName].toMap();
      engineResult.insert("name", QVariant(personName));
      engineResult.insert("engine", QVariant(Controller::engineRepr[engineName]));

      if (engineResult.contains("citations")) {
        QList<int> cits;
        QList<double> approx;
        QVariantList citVs = engineResult.value("citations").toList();
        for (QVariantList::ConstIterator citIt = citVs.constBegin();
             citIt != citVs.constEnd();
             ++citIt) {
          QVariant cit = *citIt;
          cits.append(cit.toInt());
        }
        if (engineResult.contains("approx")) {
          QVariantList approxs = engineResult.value("approx").toList();
          for (QVariantList::ConstIterator approxIt = approxs.constBegin();
               approxIt != approxs.constEnd(); ++approxIt) {
            approx.append(approxIt->toDouble());
          }
        }
        engineResult.insert("graph", this->getGraph(cits, approx));
      }

      person = engineResult;
    }
    result.insert("person", person);
  }

  QString templ;
  if (multiEngine) {
    if (multiPerson) {
      templ = "bpersons";
    } else {
      templ = "bperson";
    }
  } else {
    if (multiPerson) {
      templ = "persons";
    } else {
      templ = "person";
    }
  }
  qDebug() << result;
  emit this->complite(Controller::renderTemplate(templ, result), title);
}

void Controller::constPage(const QString &templateName) {
  QString newName = QString(templateName[0]).toUpper() + templateName.right(templateName.size() - 1);
  emit this->complite(Controller::getConstPage(templateName), newName);
  emit this->setInfo(newName);
}

// Static private
void Controller::loadTemplate(const QString &templatePath, const QString &name) {
  qDebug() << "Load template" << name << "from" << templatePath;
  try {
    QFile temp(templatePath);
    temp.open(QIODevice::ReadOnly);
    Controller::templates.insert(name, QString(temp.readAll()));
    temp.close();
  } catch (...) {
    qDebug() << "Fail load template" << templatePath << "!";
  }
}

void Controller::loadTemplates() {
  QFile config(Controller::workingPath + "templates.conf");
  config.open(QIODevice::ReadOnly);
  QStringList templatesDefenitions = QString(config.readAll()).split("\n");

  for (QStringList::ConstIterator def = templatesDefenitions.constBegin();
       def != templatesDefenitions.constEnd(); ++def) {
    const QStringList tokens = def->split("=");
    if (tokens.length() < 2) {
      continue;
    } else if (tokens.length() > 2) {
      qDebug() << "Failing read template defenition" << *def;
      continue;
    }
    Controller::loadTemplate(Controller::workingPath + tokens.at(1).trimmed(), tokens.at(0).trimmed());
  }
}#include "controller.h"

QMap<QString, QStringList> Controller::handlers;
QMap<QString, QString> Controller::engineRepr;
QMap<QString, QString> Controller::commands;
QMap<QString, QString> Controller::templates;
QString Controller::workingPath;

QRegExp Controller::replaceReg;
QRegExp Controller::applyReg;

Controller::Controller(QObject *parent) :
  QObject (parent) {
  this->locked = false;
}

Controller::~Controller() {
  for (QMap<SearchTask, QThread*>::Iterator thread = this->workers.begin();
       thread != this->workers.end(); thread++) {
    (*thread)->quit();
    (*thread)->wait();
  }
}

bool Controller::isComplite() const {
  return !this->locked;
}

bool Controller::saveResults(const QString &filePath) {
  QFile f(filePath);
  if (!f.open(QIODevice::WriteOnly)) {
    return false;
  } else {
    QStringList personsStr;
    for (QVariantMap::ConstIterator personIt = this->incompliteKeys.constBegin();
         personIt != this->incompliteKeys.constEnd(); ++personIt) {
      QStringList personStr;
      QVariantMap person = personIt.value().toMap();
      for (QVariantMap::ConstIterator engineIt = person.constBegin();
           engineIt != person.constEnd(); ++ engineIt) {
        QVariantMap engine = engineIt.value().toMap();
        QStringList engineStr;
        if (engine.contains("articles")) {
          engineStr << ("      \"articles\" : " + engine.value("articles").toString());
        }
        if (engine.contains("h_index")) {
          engineStr << ("      \"h_index\" : " + engine.value("h_index").toString());
        }
        if (engine.contains("years")) {
          engineStr << ("      \"years\" : " + engine.value("years").toString());
        }
        if (engine.contains("citations")) {
          QStringList citationsStr;
          QVariantList citations = engine.value("citations").toList();
          for (QVariantList::ConstIterator cit = citations.constBegin();
               cit != citations.constEnd(); ++cit) {
            citationsStr << cit->toString();
          }
          engineStr << ("      \"citations\" : [" + citationsStr.join(", ") + "]");
        }

        if (engine.contains("approx")) {
          QStringList citationsStr;
          QVariantList citations = engine.value("approx").toList();
          for (QVariantList::ConstIterator cit = citations.constBegin();
               cit != citations.constEnd(); ++cit) {
            citationsStr << cit->toString();
          }
          engineStr << ("      \"approx\" : [" + citationsStr.join(", ") + "]");
        }

        personStr << ("    \"" + engineIt.key() + "\" : {\n" + engineStr.join(",\n") + "\n    }");
      }
      personsStr << ("  \"" + personIt.key() + "\" : {\n" + personStr.join(",\n") + "\n  }");
    }
    f.write("{\n");
    f.write(personsStr.join(",\n").toUtf8());
    f.write("\n}");
    return true;
  }
}

// Static section
void Controller::setConsts() {
  Controller::replaceReg = QRegExp("\\[(\\w+)\\](##|%%|#|%)[{](.*)[}][?][{](.*)[}]", Qt::CaseSensitive);
  Controller::replaceReg.setMinimal(true);
  Controller::workingPath = QDir::current().absoluteFilePath("../../data/");

  Controller::handlers.insert("Google Scholar", QStringList() << "google");
  Controller::handlers.insert("Microsoft Bing", QStringList() << "ms");
  Controller::handlers.insert("Google Scholar & Microsoft Bing", QStringList() << "ms" << "google");
  Controller::handlers.insert("Test Search", QStringList() << "test1");
  Controller::handlers.insert("Double Test Search", QStringList() << "test1" << "test2");

  Controller::commands.insert("google", Controller::workingPath + "scripts/google.r");
  Controller::commands.insert("ms", Controller::workingPath + "scripts/ms.r");
  Controller::commands.insert("test1", Controller::workingPath + "scripts/test.r");
  Controller::commands.insert("test2", Controller::workingPath + "scripts/test.r");

  Controller::engineRepr.insert("google", "Google Scholar");
  Controller::engineRepr.insert("ms", "Microsoft Academic Search");
  Controller::engineRepr.insert("test1", "First Fake Search");
  Controller::engineRepr.insert("test2", "Second Fake Search");

  qRegisterMetaType<SearchTask>("SearchTask");

  Controller::loadTemplates();
}

QList<QString> Controller::engines() {
  return Controller::handlers.keys();
}

QString Controller::renderTemplate(const QString &templateName, const QVariantMap &keys) {
  QString rendered = Controller::templates.value(templateName);
  while (Controller::replaceReg.indexIn(rendered, 0) != -1) {
    const QString expr = Controller::replaceReg.cap(0);
    const QString key = Controller::replaceReg.cap(1);
    const QString type = Controller::replaceReg.cap(2);
    const QString templ = Controller::replaceReg.cap(3);
    const QString alternative = Controller::replaceReg.cap(4);

    if (!keys.contains(key)) {
      rendered.replace(expr, alternative);
      continue;
    }

    QString after = "";
    QVariant value = keys.value(key);

    if (type.length() == 2) {
      if (value.type() != QVariant::List) {
        qDebug() << "Type mismatch " << value.type() << ", List is requered.";
      } else {
        const QVariantList list = value.toList();
        for (QVariantList::ConstIterator item = list.constBegin();
             item != list.constEnd(); ++item) {
          if (type == "%%") {
            if (item->type() != QVariant::String && item->type() != QVariant::Int && item->type() != QVariant::Double) {
              qDebug() << "Type mismatch " << item->type() << "; string, int or double is requered.";
            } else {
              after += QString(templ).replace("$", item->toString());
            }
          } else {
            if (item->type() != QVariant::Map) {
              qDebug() << "Type mismatch " << item->type() << "; string, int or double is requered.";
            } else {
              after += Controller::renderTemplate(templ, item->toMap());
            }
          }
        }
      }
    } else if (type == "#") {
      if (value.type() == QVariant::Map) {
        after = Controller::renderTemplate(templ, value.toMap());
      } else {
        qDebug() << "Type mismatch " << value.type() << "; dict is requered.";
      }
    } else {
      if (value.type() == QVariant::String ||
          value.type() == QVariant::Int ||
          value.type() == QVariant::Double) {
        after = QString(templ).replace("$", value.toString());
      } else {
        qDebug() << "Type mismatch " << value.type() << "; string, int or double is requered.";
      }
    }

    rendered.replace(expr, after);
  }
  return rendered;
}

QString Controller::getGraph(const QList<int> &real, const QList<double> &approx) {
  QFile csv(Controller::workingPath + "tmp/approx.csv");
  csv.open(QIODevice::WriteOnly);
  QStringList realStr, approxStr;
  for (QList<int>::ConstIterator it = real.constBegin(); it != real.constEnd(); ++it) {
    realStr.push_back(QString::number(*it));
  }

  for (QList<double>::ConstIterator it = approx.constBegin(); it != approx.constEnd(); ++it) {
    approxStr.push_back(QString::number(*it));
  }

  csv.write(QString(realStr.join("; ") + "\n").toUtf8());
  csv.write(QString(approxStr.join("; ") + "\n").toUtf8());
  csv.close();

  QProcess worker;
  worker.setWorkingDirectory(Controller::workingPath + "tmp/");
  worker.start("sh", QStringList() << Controller::workingPath + "tmp/start.sh"
               << "1" << "1"
               << Controller::workingPath + "tmp/graph.r"
               << Controller::workingPath + "tmp/graph.log");

  worker.waitForFinished(1000);
  csv.remove();
  if (worker.exitStatus() == QProcess::NormalExit) {
    return Controller::workingPath + "tmp/graph.jpg";
  } else {
    return Controller::workingPath + "imgs/non-graph.jpg";
  }
}

QString Controller::getConstPage(const QString &name) {
  return Controller::renderTemplate(name, QVariantMap());
}

// Slots
void Controller::search(const QString &names_, const QString &type) {
  if (this->locked) {
    qDebug() << "Controller already has task!";
    return;
  } else {
    this->locked = true;
  }


  this->cleanUp();
  const QStringList engines = Controller::handlers.value(type);
  this->searchHandler = type;
  QStringList names = names_.split(",");

  this->timeout = names.size() * 1000 * 60;
  emit this->setTimeOut(this->timeout);

  for (QStringList::Iterator name = names.begin(); name != names.end(); name++) {
    (*name) = name->trimmed();
  }
  for (QStringList::ConstIterator engine = engines.constBegin();
       engine != engines.constEnd(); ++engine) {
    this->searchBy(names, *engine);
  }
  emit this->setInfo("Starting search.");
}

void Controller::searchBy(const QStringList &names, const QString &engine) {
  for (QStringList::ConstIterator name = names.constBegin();
       name != names.constEnd(); name++) {
    const SearchTask task = SearchTask(*name, engine);
    this->sheduleTask(task);
  }
}

void Controller::sheduleTask(const SearchTask &task) {
  ScriptRunner* scriptRunner = new ScriptRunner(task, Controller::workingPath + "tmp/",
                                                Controller::commands.value(task.second),
                                                this->timeout);
  QThread* thread = new QThread();
  scriptRunner->moveToThread(thread);

  connect(thread, SIGNAL(started()), scriptRunner, SLOT(search()), Qt::QueuedConnection);

  connect(thread, SIGNAL(finished()), thread, SLOT(deleteLater()), Qt::QueuedConnection);
  connect(thread, SIGNAL(finished()), scriptRunner, SLOT(deleteLater()), Qt::QueuedConnection);

  connect(scriptRunner, SIGNAL(complite(SearchTask,QVariantMap)),
          this, SLOT(addResult(SearchTask,QVariantMap)), Qt::QueuedConnection);
  connect(scriptRunner, SIGNAL(complite(SearchTask,QVariantMap)),
          thread, SLOT(quit()), Qt::QueuedConnection);

  this->workers.insert(task, thread);
  this->tasks += 1;
  this->tasksMax += 1;

  thread->start(QThread::NormalPriority);
}

void Controller::addResult(const SearchTask &task, const QVariantMap &keys) {
  const QString name = task.first;
  const QString engine = task.second;
  if (!this->incompliteKeys.contains(name)) {
    this->incompliteKeys[name] = QVariant(QVariantMap());
  }

  QVariantMap newResults = this->incompliteKeys[name].toMap();
  newResults[engine] = keys;
  this->incompliteKeys[name] = QVariant(newResults);
  this->tasks--;
  emit this->setInfo("Complite " + QString::number(this->tasksMax - this->tasks) + "/" + QString::number(this->tasksMax) + " tasks.");
  if (this->tasks <= 0) {
    this->processResults();
    this->cleanUp();
    this->locked = false;
  }
}

void Controller::cleanUp() {
  this->tasks = 0;
