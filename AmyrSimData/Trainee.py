import sys
import numpy as np

from keras.models import Sequential, Model, load_model


def loadModels(AccelModel,RotationModel,loader):
    if loader>0:
        return  AccelModel,RotationModel,loader-1
    AccelModel= load_model(r'C:\Users\aborge01.cs8451\work\Simulator\AmyrSimData\AccelModel.h5')
    #RotationModel=load_model(r'C:\Users\kokos\Desktop\DiploGit\Simulator\SimulatorTests\RotationModel.h5')
    return AccelModel,RotationModel,LOADAFTER
def processInput(input,inputs,agentInputs):
    if throw==1:
        return 0,inputs,agentInputs
    AllInputs = input.split(sep=':')
    newTimeStep=[]
    for aString in AllInputs:
        if len(aString)==0:
            pass
        else:
            arr = np.fromstring(aString, sep=';')
            arr = np.reshape(arr, (1, int(features/timesteps)))
            #arr[:, :60] = np.sqrt(arr[:,:60])

            #arr = np.reshape(arr, (1, features+1))
            #arr=np.delete(arr,features-1,1)
            if len(inputs) == 0:
                inputs = arr
            else:
                inputs = np.append(inputs, arr, 0)
            newTimeStep.append(arr)
    newTimeStep=np.reshape(newTimeStep,[agents,int(features/timesteps)])
    agentInputs = np.delete(agentInputs, np.s_[:(int(features/timesteps))], axis=1)
    agentInputs = np.append(agentInputs, np.array(newTimeStep), axis=1)
    inputs=agentInputs
    return 0,inputs,agentInputs

def predict(inputs):
    Accel=AccelModel.predict([inputs], batch_size=8, verbose=0)
    #Rot=RotationModel.predict([inputs], batch_size=8, verbose=0)
    #Rot = np.reshape(Rot, (int(Rot.size / 11), 11))
    #Rot=np.argmax(Rot,axis=1)
    #Rot = np.reshape(Rot, (Rot.size, 1))
    stringToRet=''
    for acc in (Accel):
        stringToRet=stringToRet+str(acc[0])+','+str(acc[1])+':'
    return stringToRet
def predictZeros():
    agents=len(inputs)
    stringToRet=''
    for i in range(int(agents)):
        stringToRet=stringToRet+str(0)+','+str(0)+':'
    return stringToRet

throw = 1
print('dasfasfa', end='\n')
LOADAFTER=1500
timesteps=4
features=6*11+1
features*=timesteps
in1='0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0:'
in1='0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0.02791715,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-0.2648614,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0.5981368,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-0.1184356,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-0.1923338,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0.1339654,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0.04495089,4:0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,-0.01219456,4:'
in1=in1[:-1]
inputs=np.array([])
loader=1500+LOADAFTER
loader=10
AccelModel=None
RotationModel=None
ok = sys.stdin.readline()

agents=1
agentInputs=[]
for i in range(agents):
    agentInputs.append([0]*features)

agentInputs=np.reshape(agentInputs,[agents,features])

while True:
    ok = sys.stdin.readline()
    ok = ok[:-2]
    #print(ok)
    #ok=in1
    #ok=''
    [throw, inputs,agentInputs] = processInput(ok, inputs,agentInputs)
    [AccelModel,RotationModel,loader]=loadModels(AccelModel,RotationModel,loader)
    if AccelModel!=None:
        print(predict(inputs),end='\n')
    else:
        print(predictZeros(),end='\n')
    sys.stdout.flush()
    inputs = np.array([])
