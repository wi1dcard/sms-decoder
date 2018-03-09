import serial
import time
import sys

ser = serial.Serial("/dev/tty.usbserial", 9600, timeout = 5)
def main():
    while True:
        recv = ser.readline()
        print(recv.decode('utf-8'))
        sys.stdout.flush()
    
if __name__ == '__main__':
    try:
        main()
    except KeyboardInterrupt:
        if ser != None:
            ser.close()
