## Huffman Encoding Pseudocode
    Procedure HuffmanEncoding (PQ)          //PQ is the priority Queue with Letters		                       
           S = PQ.Size			       //  and Frequencies set it will be custom built 
           while S is not equal to 1 do
                  N = new Node ( ) 
                  N.left = PQ.pop
                  N.right = PQ.pop
                  N.frequency = N.left.frequency + N.right.frequency
                  PQ.Insert (N)
	              S = PQ.Size
           end while
     return  PQ.Top

## Huffman Decoding Pseudocode 
      Procedure HuffmanDecoding (root, in)         //Root is the Huffman binary tree root and in is 
          current = root			                // the bit-stream to be decoded    
           while true do                                                
                  bit = in.readBit()   
                  if bit is equal to 0
                      current = current.left
                  if bit is equal to 1
                      current = current.right
                  if current.right equal to NULL OR current.left equal to NULL
	                break
           end while
        return current.data

