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


import select


def sock_connection(sock, host,models):
    while True:

        print('waiting')
        newsock.setblocking(1)
        data = newsock.recv(1009)
        print('kyk')

        newsock.setblocking(0)

        ready = select.select([newsock], [], [], 0.05)
        if ready[0]:
            data += newsock.recv(2000)
        ready = select.select([newsock], [], [], 0.01)
        if ready[0]:
            data += newsock.recv(2000)



                # data += newsock.recv(14096)
       # data += newsock.recv(14096)

        data1 = data.decode('utf-8')
        #print(data1)

        lines = data1.splitlines()
        print(len(lines))
        arr = np.fromstring(data1, sep=',')
        #arr = np.reshape(arr, (1, Models[0].inputFeautures))
        arr = np.reshape(arr, (1, 9))

        for a in lines:
            o = np.fromstring(a, sep=',')
            #o = np.reshape(o, (1, Models[0].inputFeautures))
            o = np.reshape(o, (1, 9))

            arr = np.append(arr, o, 0)
        arr = np.delete(arr, np.s_[0:6], 1)
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
        self.name='0LinearMyData'+str(layerNum)+'Layers'+str(LayerStandardWidth)+'Width'+str(dropoutPercentage)+'dropout'+str(Xval)+'Xval'
        self.layers=layerNum
        self.X = np.delete(X, [features,features+1], 1)
        #self.X = np.delete(X, [features,features+1,features-2,features-1], 1)

        self.Y = np.delete(Y, np.s_[0:features], 1)
        #self.Y = np.delete(Y, np.s_[0:features+2], 1)

        #self.AccurateY= np.array(self.Y, copy=True)
        #self.AccurateY= self.AccurateY[10000:, :]
        if YVal==1:
            self.giveAModelNum1()
        else:
            self.giveAModelNum()


    def giveAModelNum1(self):
        input1 = Input(shape=(self.inputFeautures,), name='input1',)
        # input2=Input(shape=(84,),name='input2')

        # Together=concatenate([input1,input2])
        i = self.layers
        oneMoreLayer=(BatchNormalization())(input1)
        oneMoreLayer = Dense(3 ,kernel_initializer='normal', activation='relu',kernel_regularizer=regularizers.l2(0.1))(oneMoreLayer)
        #oneMoreLayer = Dropout(0.5)(oneMoreLayer)
        #oneMoreLayer = Dense(features*2,kernel_initializer='normal', activation='relu',kernel_regularizer=regularizers.l2(0.05))(oneMoreLayer)
        #oneMoreLayer = Dropout(0.5)(oneMoreLayer)
        #oneMoreLayer = Dense(32*2,kernel_initializer='normal', activation='relu')(oneMoreLayer)

        outX = Dense(1,kernel_initializer='normal', name='outX')(oneMoreLayer)

        model = Model(inputs=[input1], outputs=[outX])

        model.compile(loss='mse',
                      optimizer='adam')

        self.model=model

        plot_model(model, self.name+'Arch.png', True)
       # AngleModel.fit([X2[:2624, :]], [NewY[:2624, :]],
               #        epochs=1500, batch_size=2048, validation_split=0.05)


    def giveAModelNum(self):
        input1 = Input(shape=(self.inputFeautures,), name='input1',)
        # input2=Input(shape=(84,),name='input2')

        # Together=concatenate([input1,input2])
        i = self.layers
        oneMoreLayer=(BatchNormalization())(input1)
        oneMoreLayer = Dense(3 ,kernel_initializer='normal', activation='relu',kernel_regularizer=regularizers.l2(0.1))(oneMoreLayer)
        #oneMoreLayer = Dropout(0.75)(oneMoreLayer)
        #oneMoreLayer = Dense(features,kernel_initializer='normal', activation='relu')(oneMoreLayer)

        outX = Dense(1,kernel_initializer='normal', name='outX')(oneMoreLayer)

        model = Model(inputs=[input1], outputs=[outX])

        model.compile(loss='mse',
                      optimizer='adam')

        self.model=model

        plot_model(model, self.name+'Arch.png', True)
       # AngleModel.fit([X2[:2624, :]], [NewY[:2624, :]],
               #        epochs=1500, batch_size=2048, validation_split=0.05)


    def trainMeSenpai(self,epochs):

        history=self.model.fit([self.X[:, :]], [self.Y[:, 0]],
                  epochs=epochs, batch_size=2048,validation_split=0.2,verbose=1)
        self.model.save(self.name+'Model.h5')

    def trainMeSenpaiY(self,epochs):

        history = self.model.fit([self.X[:, :]], [self.Y[:, 1]],
                                 epochs=epochs, batch_size=2048, validation_split=0.2, verbose=1)
        self.model.save(self.name + '1Model.h5')



    def LoadMeSenpai(self):

        self.model = load_model(self.name+'Model.h5')
    def LoadMeSenpaiY(self):

        self.model = load_model(self.name+'1Model.h5')

    def predictArray(self,mymodel1,mymodel2,X):


        self.doThis(X)
        X=self.X
        arr = mymodel1.model.predict([self.X[:, :]], batch_size=8, verbose=1)
        arr1 = mymodel2.model.predict([self.X[:, :]], batch_size=8, verbose=1)
        self.Result =np.append(arr,arr1,1)
        return  self.Result


    def predict(self):
        arr = self.model.predict([self.X[10000:, :]], batch_size=32, verbose=1)
        arr=np.append(arr,self.AccurateY,1)
        for a in arr:
            print(a)

        i=9
        while i>0:
            print('------------------------------------------------')
            i-=1

    '''def predictY(self):
        arr = self.model.predict([self.X[10000:, :]], batch_size=32, verbose=1)
        arr=np.append(arr,self.AccurateYy,1)
        for a in arr:
            print(a)'''


np.random.seed(42)
X = genfromtxt('newd23C.csv', delimiter=',')
Y = genfromtxt('newd23C.csv', delimiter=',')
X1=np.delete(X,0,0)
np.random.shuffle(X1)

Y1=X1

DefaultWidth=32
DefaultDropout=0
DefaultLayers=7
Models=[]
i=0
layers=DefaultLayers
features=3
Multi=True
dropout=DefaultDropout#0 0.1 0.2 0.05 0.15 0.25
epochs=1500
width=DefaultWidth
use=[]
numOfSeparation=0
Models.append(MyModel(X1, Y1, layers, features, Multi, width, dropout, epochs,-8-(7-numOfSeparation)*0.125,0))
Models[i].trainMeSenpai(550)
#Models[i].LoadMeSenpai()
#Models[i].predict()


i+=1
Models.append(MyModel(X1, Y1, layers, features, Multi, width, dropout, epochs,-8-(7-numOfSeparation)*0.125,1))
Models[i].trainMeSenpaiY(550)
#Models[i].LoadMeSenpaiY()
#Models[i].predictY()


print('server')
serversocket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

# now connect to the web server on port 80 - the normal http port
#s.connect(("www.python.org", 80))
serversocket.bind(('127.0.0.1', 1002))
# become a server socket
serversocket.listen(5)

while True:

        newsock,addr = serversocket.accept()
       # data=newsock.recv(1024)
        #print(data.decode('utf-8'))
       # data='fas'
        #newsock.send(data.encode('utf-8'))
        #newsock.timeout=20
        sock_connection(newsock,addr,Models)
        #thread = Thread(target=sock_connection, args=[newsock,addr,Models])
        #thread.start()

