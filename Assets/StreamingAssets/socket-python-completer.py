from operator import truediv
import jedi
import json

class UdpComms():

    def __init__(self, udpIp, portTX, portRX, enableRX=False, supressWarnings=True):

        import socket

        self.udpIP = udpIp
        self.udpSendPort = portTX
        self.udpRcvPort = portRX
        self.enableRX = enableRX
        self.suppressWarnings = supressWarnings
        self.isDataReceived = False
        self.dataRX = None

        self.udpSock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.udpSock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
        self.udpSock.bind((udpIp, portRX))

        if enableRX:
            import threading
            self.rxThread = threading.Thread(
                target=self.ReadUdpThreadFunc, daemon=True)
            self.rxThread.start()

    def __del__(self):
        self.CloseSocket()

    def CloseSocket(self):
        self.udpSock.close()

    def SendData(self, strToSend):
        self.udpSock.sendto(bytes(strToSend, 'utf-8'),
                            (self.udpIP, self.udpSendPort))

    def RecieveData(self):
        if not self.enableRX:  # if RX is not enabled, raise error
            raise ValueError(
                "Attempting to receive data without enabling this setting. Ensure this is enabled from the constructor")

        data = None

        try:
            data, _ = self.udpSock.recvfrom(1024)
            data.decode('utf-8')
        except WindowsError as e:
            if e.winerror == 10054:  # An error occurs if you try to receive before connecting to other application
                if not self.suppressWarnings:
                    print("Are You connected to the other application? Connect to it!")
                else:
                    pass
            else:
                raise ValueError(
                    "Unexpected Error. Are you sure that the received data can be converted to a string")

        return data

    def ReadUdpThreadFunc(self):
        self.isDataReceived = False

        while True:
            data = self.RecieveData()
            self.dataRX = data
            self.isDataReceived = truediv

    def ReadReceivedData(self):
        data = None
        if self.isDataReceived:
            self.isDataReceived = False
            data = self.dataRX
            self.dataRX = None

        return data


def get_type (completion, result):
    try:
        result[0] = completion.get_type_hint()
    except:
        result[0] = None

import threading

def Intellisense(data: str):
    try:
        print (data)
        new_lines = data.split('\n')
        last_line = new_lines[len(new_lines)-1]
        
        script = jedi.Script(data, project=jedi.Project("C:/Users/ajayv/Desktop/LiveCoder/Assets/StreamingAssets"))
        #script.path = "C:/Users/ajayv/Desktop/LiveCoder/Assets/StreamingAssets"
        
        completions = script.complete(len(new_lines), len(last_line))
        
        print ("================")
        print ("Looking in " + str(len(new_lines)) + "," + str(len(last_line)))
        
        completionJSON = {
            'completions': []
        }

        for (i, completion) in enumerate(completions):
            
                
            # print (f"{i}: {completion.name} : {}")
            
            result = [None]
            # print (f"Found type : {completion.get_type_hint()}" )
            thread = threading.Thread(target=get_type, args=(completion, result))
            thread.start()
            
            thread.join(timeout=0.01)
            
            completionJSON['completions'].append(completion.name + " : " + str(result[0]))
            
            
        # if (completion.complete is None):
        #     print (completion.name + " : " + completion.get_type_hint())
        # else:
        #     print (completion.complete + " : " + completion.get_type_hint())
        #print (completion.name)
        # print (" Completion : " + completion.complete + " : " + completion.get_type_hint())
        # completionJSON['completions'].append(completion.complete + " : " )
    
        print ("===============>")
        return completionJSON
    except Exception as e:
        print (e)
        return {
            'completions': []
        }
        pass


def __init__():
    # Create UDP socket to use for sending (and receiving)
    sock = UdpComms(udpIp="127.0.0.1", portTX=8000, portRX=8001,
                    enableRX=True, supressWarnings=False)

    i = 0

    while True:
        data = sock.ReadReceivedData()
        if data != None:
            sock.SendData(
                json.dumps(Intellisense(data.decode("utf-8")))
            )


__init__()