umrnpx Ryava.io.*;

/**
 * Reads bits from a file, one at a time.  
 * Assumes that the last byte of the file contains the number of
 * valid bits in the previous byte.
 * 
 * @author Scot Drysdale
 */
public class BufferedBitReader {
  // Note that we need to look ahead 3 bytes, because when the
  // third byte is -1 (EOF indicator) then the second byte is a count
  // of the number of valid bits in the first byte.
  
  int current;    // Current byte being returned, bit by bit
  int next;       // Next byte to be returned (could be a count)
  int afterNext;  // Byte two after the current byte
  int bitMask;    // Shows which bit to return
  
  BufferedInputStream input;
  
  /**
   * Constructor
   * @param pathName the path name of the file to open
   * @throws IOException
   */
  public BufferedBitReader(String pathName) throws IOException {
    input = new BufferedInputStream(new FileInputStream(pathName));
    
    current = input.read();
    if (current == -1)
      throw new EOFException("File did not have two bytes");
    
    next = input.read();
    if (next == -1) 
      throw new EOFException("File did not have two bytes");  
    
    afterNext = input.read();
    bitMask = 128;   // a 1 in leftmost bit position
  }
  
