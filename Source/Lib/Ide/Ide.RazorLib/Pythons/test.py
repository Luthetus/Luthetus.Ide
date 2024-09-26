# https://biriukov.dev/docs/fd-pipe-session-terminal/4-terminals-and-pseudoterminals/

import os
import time
import sys

print(f"parent: {os.getpid()}")

ptmx, secondary = os.openpty()

pid = os.fork()
if pid == 0:
	print(f"child: {os.getpid()}")
	os.close(ptmx)
	os.setsid()
	name = os.ttyname(secondary)
	print(name)
	s = os.open(name, os.O_RDWR)
	os.dup2(s, 0)
	os.dup2(s, 1)
	os.dup2(s, 2)
	os.close(secondary)
	os.close(s)
	with open('/tmp/file.txt', "w") as f:
    	for l in sys.stdin:
        	f.write(l)
        	f.flush()
	time.sleep(999999)
else:
	os.close(secondary)
	os.write(ptmx, b"text\n")
	os.waitpid(-1,0)
