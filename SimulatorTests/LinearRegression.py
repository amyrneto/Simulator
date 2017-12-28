import numpy as np
import h5py
import math

from keras.models import Sequential, Model, load_model
from keras.layers import Dense, Dropout, Activation, Flatten, BatchNormalization,Input
from keras.utils import np_utils
from keras import regularizers
from numpy import genfromtxt
from matplotlib import pyplot as plt
from keras.utils import plot_model
from keras.callbacks import EarlyStopping
import  socket
from threading import  Thread
import  time


def sock_connection(sock, host,models):
    while True:


        print('waiting')
        data = newsock.recv(100)
        print('kyk')

        data += newsock.recv(4096)

        data1 = data.decode('utf-8')
        print(data1)

        lines = data1.splitlines()
        print(lines)
        arr = np.fromstring(data1, sep=',')
        arr = np.reshape(arr, (1, 9))
        for a in lines:
            print(a)
            o = np.fromstring(a, sep=',')
            print(o)
            o = np.reshape(o, (1, 9))

            arr = np.append(arr, o, 0)

        arr = np.delete(arr, 0, 0)
        data=retString(arr)
        #time.sleep(1)
        newsock.send(data.encode('utf-8'))

    return


def retString(arr):

    toret=''

    Models[0].predictArray(Models[0],Models[1], arr)
    for o in Models[0].Result:
        toret=toret+str(o[0])+','+str(o[1])+'\n'

    print(toret)
    return  toret
        # print()


class MyModel:
    'Common base class for all employees'
    empCount = 0


    def doThis(self,X):

        self.X=X





    def __init__(self, X, Y,layerNum,features,NodesMultipliedByLayers,LayerStandardWidth,dropoutPercentage,epochs,Xval,YVal):
        self.epochs=epochs
        self.NodesMultipliedByLayers=NodesMultipliedByLayers
        self.LayerStandardWidth=LayerStandardWidth
        self.dropoutPercentage=dropoutPercentage
        self.inputFeautures=features
        self.name='5Linear'+str(layerNum)+'Layers'+str(LayerStandardWidth)+'Width'+str(dropoutPercentage)+'dropout'+str(Xval)+'Xval'
        self.layers=layerNum
        self.X = np.delete(X, [9, 10], 1)
        self.Y = np.delete(Y, np.s_[0:9], 1)
        self.Yy = np.array(self.Y, copy=True)
        self.Y = np.delete(self.Y, 1, 1)
        self.Yy = np.delete(self.Yy, 0, 1)
        self.AccurateY= np.array(self.Y, copy=True)
        self.AccurateYy=np.array(self.Yy, copy=True)
        self.AccurateY= self.AccurateY[2124:, :]
        self.AccurateYy= self.AccurateYy[2124:, :]
        self.giveAModelNum()



    def giveMeAngle(self,x,y):
        toRet=[]
        for i in self.X:
            toRet.append((math.atan2(i[x], i[y])))
        opas=np.array(toRet)
        opas=np.reshape(opas,[len(opas),1])
        return opas

    def combineValues(self):
        i = 0
        j = 0
        z = 0
        while i < 28:
            while j < 12:
                Y2indices = (self.Y == i) & (self.Yy == j)
                self.NewY[Y2indices] = z
                j += 1
                z += 1
            j = 0
            i += 1

        self.NewYCopy = np.array(self.NewY, copy=True)
        self.NewY = np_utils.to_categorical(self.NewY, 336)
        #self.differentProbabilities4()


    def giveAModelNum(self):
        input1 = Input(shape=(9,), name='input1',)
        # input2=Input(shape=(84,),name='input2')

        # Together=concatenate([input1,input2])
        i = self.layers
        oneMoreLayer=(BatchNormalization())(input1)
        oneMoreLayer = Dense(self.LayerStandardWidth * (i - 3),kernel_initializer='normal', activation='relu')(oneMoreLayer)
        oneMoreLayer = Dense(self.LayerStandardWidth * (i -4),kernel_initializer='normal', activation='relu')(oneMoreLayer)
        oneMoreLayer = Dense(self.LayerStandardWidth * (i - 5),kernel_initializer='normal', activation='relu')(oneMoreLayer)

        outX = Dense(1,kernel_initializer='normal', name='outX')(oneMoreLayer)

        model = Model(inputs=[input1], outputs=[outX])

        model.compile(loss='mse',
                      optimizer='adam')

        self.model=model

        plot_model(model, self.name+'Arch.png', True)
       # AngleModel.fit([X2[:2624, :]], [NewY[:2624, :]],
               #        epochs=1500, batch_size=2048, validation_split=0.05)


    def trainMeSenpai(self,epochs):

        history=self.model.fit([self.X[:2124, :]], [self.Y[:2124, :]],
                  epochs=epochs, batch_size=2048,validation_split=0.2,verbose=1)
        self.model.save(self.name+'Model.h5')
    def trainMeSenpaiY(self,epochs):

        history=self.model.fit([self.X[:2124, :]], [self.Yy[:2124, :]],
                  epochs=epochs, batch_size=2048,validation_split=0.2,verbose=1)
        self.model.save(self.name+'Model.h5')



    def LoadMeSenpai(self):

        self.model = load_model(self.name+'Model.h5')

    def predictArray(self,mymodel1,mymodel2,X):


        self.doThis(X)
        X=self.X
        arr = mymodel1.model.predict([self.X[:, :]], batch_size=8, verbose=1)
        arr1 = mymodel2.model.predict([self.X[:, :]], batch_size=8, verbose=1)
        self.Result = np.append(arr, arr1, 1)
        return  self.Result


    def predict(self):
        arr = self.model.predict([self.X[2124:, :]], batch_size=32, verbose=1)
        arr=np.append(arr,self.AccurateY,1)
        for a in arr:
            print(a)

        i=9
        while i>0:
            print('------------------------------------------------')
            i-=1
    def predictY(self):
        arr = self.model.predict([self.X[2124:, :]], batch_size=32, verbose=1)
        arr=np.append(arr,self.AccurateYy,1)
        for a in arr:
            print(a)


np.random.seed(42)
X = genfromtxt('newd2.csv', delimiter=',')
Y = genfromtxt('newd2.csv', delimiter=',')
X1=np.delete(X,0,0)
np.random.shuffle(X1)

Y1=X1

DefaultWidth=32
DefaultDropout=0
DefaultLayers=7
Models=[]
i=0
layers=DefaultLayers
features=9
Multi=True
dropout=DefaultDropout#0 0.1 0.2 0.05 0.15 0.25
epochs=1500
width=DefaultWidth
use=[]
numOfSeparation=0
Models.append(MyModel(X1, Y1, layers, features, Multi, width, dropout, epochs,-8-(7-numOfSeparation)*0.125,-4-(7-numOfSeparation)*0.125))
Models[i].trainMeSenpai(870)
Models[i].predict()

i+=1
Models.append(MyModel(X1, Y1, layers, features, Multi, width, dropout, epochs,-8-(7-numOfSeparation)*0.125,-4-(7-numOfSeparation)*0.125))
Models[i].trainMeSenpaiY(870)
Models[i].predictY()
#Models[i].LoadMeSenpai()


print('server')
serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# now connect to the web server on port 80 - the normal http port
#s.connect(("www.python.org", 80))
serversocket.bind(('192.168.0.4', 1002))
# become a server socket
serversocket.listen(5)

while True:



        newsock,addr = serversocket.accept()
       # data=newsock.recv(1024)
        #print(data.decode('utf-8'))
       # data='fas'
        #newsock.send(data.encode('utf-8'))
        sock_connection(newsock,addr,Models)
        #thread = Thread(target=sock_connection, args=[newsock,addr,Models])
        #thread.start()

