## Huffman Encoding Pseudocode
    Procedure HuffmanEncoding (PQ)          //PQ is the priority Queue with Letters		                       
           S = PQ.Size			         and Frequencies set it will be custom built 
           while S is not equal to 1 do
                  N = new Node ( ) 
                  N.left = PQ.pop
                  N.right = PQ.pop
                  N.frequency = N.left.frequency + N.right.frequency
                  PQ.Insert (N)
	              S = PQ.Size
           end while
     return  PQ.Top
