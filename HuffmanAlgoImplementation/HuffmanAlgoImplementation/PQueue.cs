using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanAlgoImplementation
{
    class PQueue
    {
        public class cNode
        {
            public char value;
            public int frequency;
            public cNode leftZero;
            public cNode rightOne;
            public cNode next;
            public cNode()
            {
                next = null;   //empty construcor
                leftZero = null;
                rightOne = null;
            }
            public cNode(char v)
            {
                value = v;   //value constructor
            }

            public char getValue()
            {
                return value;
            }

            public void setValue(char v)
            {
                value = v;
            }

            public void print()
            {
                Console.WriteLine(value);
            }
        }

        public class PriorityQueue
        {
            cNode tail;
            cNode top;
            public int count;

            public PriorityQueue()
            {
                tail = null;
                top = null;
                count = 0;
            }
            public PriorityQueue(cNode ptr)
            {
                tail = top = ptr;
                top.next = null;
                tail.next = null;
                ptr = null;
                ++count;
            }


            public PriorityQueue insertWithPriority(cNode ptr)
            {
                if (top == null)     //Means if top is empty
                {
                    top = tail = ptr;
                    tail.next = ptr = null;
                    top.next = null;
                    ++count;
                    return this;
                }
                else
                {
                    if (top.frequency > ptr.frequency)
                    {                                                 //IN order to enter at first means of highest priority = 1
                        ptr.next = top;
                        top = ptr;
                        ptr = null;
                        ++count;
                        return this;
                    }
                    else if (tail.frequency <= ptr.frequency)
                    {
                        if (top != null)
                        {
                            tail.next = ptr;
                            tail = tail.next;           //Now once its empty this will not work cuz top is pointing tward nothing
                        }
                        else
                        {
                            top = tail = ptr;
                        }

                        tail.next = null;
                        ptr = null;
                        ++count;
                        return this;                 //to enter at last means lowest priority = 1

                    }
                    else
                    {
                        cNode rPtr;
                        rPtr = top;                        //to enter in middle
                        cNode pPtr = top;
                        while (rPtr.frequency <= ptr.frequency)
                        {
                            pPtr = rPtr;
                            rPtr = rPtr.next;
                        }
                        ptr.next = pPtr.next;
                        pPtr.next = ptr;
                        ptr = null;
                        ++count;

                    }
                    return this;
                }
            }

            public cNode remove()
            {
                if (top.next != null)
                {
                    cNode ptr;
                    ptr = top;
                    top = top.next;
                    ptr.next = null;
                    --count;
                    return ptr;
                }
                else
                {
                    cNode ptr;
                    ptr = top;
                    top = top.next;
                    ptr.next = null;
                    tail = null;
                    --count;
                    return ptr;
                }

            }

            bool isNotEmpty()
            {
                if (top == null)
                    return false;
                else
                    return true;
            }
            bool isEmpty()
            {
                if (top == null)
                    return true;
                else
                    return false;
            }


            public cNode Top()
            {
                return top;
            }

            public void print()
            {
                cNode ptr = top;
                if (isNotEmpty())
                {
                    while (ptr != null)
                    {
                        Console.WriteLine("value: {0}, frequency: {1}", ptr.getValue(), ptr.frequency);
                        ptr = ptr.next;
                    }
                }
                else
                    Console.WriteLine("Empty");
            }

        }
    }
}

