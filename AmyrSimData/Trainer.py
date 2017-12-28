import sys
import numpy as np
import h5py
import math
import time

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
import select


def initAccelModel():
    input1 = Input(shape=(features,), name='input1', )
    oneMoreLayer = (BatchNormalization())(input1)
    oneMoreLayer = Dense(features, kernel_initializer='normal', activation='relu',
                         )(oneMoreLayer)
    #oneMoreLayer = Dropout(0.5)(oneMoreLayer)
    #oneMoreLayer = Dense(features*3, activation='relu')(oneMoreLayer)
    #oneMoreLayer = Dropout(0.25)(oneMoreLayer)
    #oneMoreLayer = Dense(features,kernel_initializer='normal', activation='relu')(oneMoreLayer)
    outX = Dense(1, kernel_initializer='normal',name='outX',activation='sigmoid')(oneMoreLayer)
    model = Model(inputs=[input1], outputs=[outX])
    model.compile(loss='mse',
                  optimizer='adam')
    return model
def initRotationModel():
    input1 = Input(shape=(features,), name='input1', )
    oneMoreLayer = (BatchNormalization())(input1)
    oneMoreLayer = Dense(features*7, kernel_initializer='normal', activation='relu',
                         )(input1)
    oneMoreLayer = Dropout(0.7)(oneMoreLayer)
    oneMoreLayer = Dense(features*8, activation='relu')(oneMoreLayer)
    oneMoreLayer = Dropout(0.7)(oneMoreLayer)
    oneMoreLayer = Dense(features*6, activation='relu')(oneMoreLayer)
    outX = Dense(1, kernel_initializer='normal', name='outX')(oneMoreLayer)
    model = Model(inputs=[input1], outputs=[outX])
    model.compile(loss='mse',
                  optimizer='adam')
    return model
def processInput(input,inputs):
    if throw==1:
        return 0,inputs
    arr = np.fromstring(input, sep=',')
    arr = np.reshape(arr, (1,features+outputs))
    if len(inputs)==0:
        inputs=arr
    else:
        inputs=np.append(inputs,arr,0)
    return 0,inputs

def decideWhetherToTrain(inputs):
    if len(inputs)>MAXTRAINAGAIN:
        return 1
    return 0
def trainModels(inputs,TrainAgain):
    if decideWhetherToTrain(inputs)==0:
        return TrainAgain -1,inputs
    if TrainAgain>0:
        return TrainAgain - 1,inputs
    AccelModel.fit(inputs[:,0:features], [inputs[:, features:features+1]],
              epochs=120, batch_size=100000, verbose=1,validation_split=0.2)
    AccelModel.save('AccelModel.h5')
    AccelModel2.fit(inputs[:, 0:features], [inputs[:, features+1:features+2]],
                   epochs=120, batch_size=100000, verbose=1, validation_split=0.2)
    AccelModel2.save('AccelModel2.h5')
    len1=int(len(inputs)/30)
    inputs=inputs[len1:]
    return  MAXTRAINAGAIN,inputs

data = genfromtxt('outputFile.csv', delimiter=';')
#width=data[0,0]
#height=data[0,1]
#X=data[0,2]
#Z=data[0,3]
#Should be width*height
features=6*11+1
data=data[1:,:]
#IF timestamp is enabled,uncomment me.
#data=data[:,1:]
outputs=data[:,features:]
outputs=np.delete(outputs,1,axis=1)
data=data[:,:features]
timesteps=4
processedData=[]
for i in range(len(data)-timesteps):
    line=data[i]
    for j in range(1,timesteps):
        line=np.append(line,data[i+j],axis=0)
    line=np.append(line,outputs[i+timesteps-1])
    processedData.append(line)


processedData=np.array(processedData)
#MAXTRAINAGAIN=1200
MAXTRAINAGAIN=0

throw=1
#inputs=np.array([])


#Normalize output range 0-1
features=features*timesteps

print(np.min(processedData[:,features:],axis=0))
print(np.max(processedData[:,features:],axis=0))

processedData[:,features:]=(processedData[:,features:]-np.min(processedData[:,features:]))/((np.max(processedData[:,features:]))- np.min(processedData[:,features:]))
print(np.min(processedData[:,features:]))
print(np.max(processedData[:,features:]))

outputs=2
trainAgain=0
AccelModel=initAccelModel()
AccelModel2=initAccelModel()
[trainAgain,inputs]=trainModels(processedData,trainAgain)
time.sleep(10)

print("Now Finished")