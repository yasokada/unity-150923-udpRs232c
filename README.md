# unity-150923-udpRs232c

- developed on Unity 5.1.3-f1 on MacOS X 10.8.5
- Encoding: ASCII
//- delay(msec) at out-to-in-relay
 
- UDP / RS-232C converter
- can communicate between UDP and RS-232C (9600 8N1)
      + 1. send from udp first (to obtain port number to return)
      + 2. send from RS-232C to udp
      + 3. continue step 2 (or from step 1)
