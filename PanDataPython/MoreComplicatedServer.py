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
        #print(data1)

        lines = data1.splitlines()
        #print(lines)
        arr = np.fromstring(data1, sep=',')
        arr = np.reshape(arr, (1, 9))
        for a in lines:
            #print(a)
            o = np.fromstring(a, sep=',')
            #print(o)
            o = np.reshape(o, (1, 9))

            arr = np.append(arr, o, 0)

        arr = np.delete(arr, 0, 0)
        data=retString(arr)
        #time.sleep(1)
        newsock.send(data.encode('utf-8'))

    return


def retString(arr):
    numOfSeparation = 0
    i=0
    toret=''

    while numOfSeparation < 8:
        Models[i].predictArray(8 + (7 - numOfSeparation) * 0.125 + 0.5, 4 + (7 - numOfSeparation) * 0.125 + 0.5,arr)
        numOfSeparation += 1
        i+=1

    for p, j, p1, j1, p2, j2, p3, j3 in zip(Models[use[0]].Result, Models[use[1]].Result, Models[use[2]].Result,
                                                  Models[use[3]].Result, Models[use[4]].Result, Models[use[5]].Result,
                                                  Models[use[6]].Result, Models[use[7]].Result
                                                  ):
        print((p + j + p1 + j1 + p2 + j2 + p3 + j3) / 8)
        a=(p + j + p1 + j1 + p2 + j2 + p3 + j3) / 8
        toret=toret+str(a[0])+','+str(a[1])+'\n'

    print(toret)
    return  toret
        # print()


class MyModel:
    'Common base class for all employees'
    empCount = 0


    def doThis(self,X):

        self.X=X
        '''
        X=np.append(X,self.giveMeAngle(0,1),1)
        X=np.append(X,self.giveMeAngle(2,3),1)
        X=np.append(X,self.giveMeAngle(4,5),1)
        X=np.append(X,self.giveMeAngle(7,8),1)

        x=np.power(X[:,0],2)+np.power(X[:,1],2)


        X=np.append(X,np.reshape(np.power(X[:,0],2)+np.power(X[:,1],2),[len(X),1]),1)
        X=np.append(X,np.reshape(np.power(X[:,2],2)+np.power(X[:,3],2),[len(X),1]),1)
        X=np.append(X,np.reshape(np.power(X[:,4],2)+np.power(X[:,5],2),[len(X),1]),1)
        X=np.append(X,np.reshape(np.power(X[:,7],2)+np.power(X[:,8],2),[len(X),1]),1)

        X=np.append(X,np.reshape(np.power(X[:,6],2),[len(X),1]),1)
        self.X=X'''





    def __init__(self, X, Y,layerNum,features,NodesMultipliedByLayers,LayerStandardWidth,dropoutPercentage,epochs,Xval,YVal):
        self.epochs=epochs
        self.NodesMultipliedByLayers=NodesMultipliedByLayers
        self.LayerStandardWidth=LayerStandardWidth
        self.dropoutPercentage=dropoutPercentage
        self.inputFeautures=features
        self.name='5NormalMoreCompXYTwoOutputsIdea'+str(layerNum)+'Layers'+str(LayerStandardWidth)+'Width'+str(dropoutPercentage)+'dropout'+str(Xval)+'Xval'
        self.layers=layerNum
        self.X = np.delete(X, [9, 10], 1)
        self.Y = np.delete(Y, np.s_[0:9], 1)
        self.Yy = np.array(self.Y, copy=True)
        self.Y = np.delete(self.Y, 1, 1)
        self.Yy = np.delete(self.Yy, 0, 1)
        self.AccurateY= np.array(self.Y, copy=True)
        '''
        self.X=np.append(self.X,self.giveMeAngle(0,1),1)
        self.X=np.append(self.X,self.giveMeAngle(2,3),1)
        self.X=np.append(self.X,self.giveMeAngle(4,5),1)
        self.X=np.append(self.X,self.giveMeAngle(7,8),1)

        x=np.power(self.X[:,0],2)+np.power(self.X[:,1],2)


        self.X=np.append(self.X,np.reshape(np.power(self.X[:,0],2)+np.power(self.X[:,1],2),[len(self.X),1]),1)
        self.X=np.append(self.X,np.reshape(np.power(self.X[:,2],2)+np.power(self.X[:,3],2),[len(self.X),1]),1)
        self.X=np.append(self.X,np.reshape(np.power(self.X[:,4],2)+np.power(self.X[:,5],2),[len(self.X),1]),1)
        self.X=np.append(self.X,np.reshape(np.power(self.X[:,7],2)+np.power(self.X[:,8],2),[len(self.X),1]),1)

        self.X=np.append(self.X,np.reshape(np.power(self.X[:,6],2),[len(self.X),1]),1)'''


        self.AccurateYy=np.array(self.Yy, copy=True)
        self.AccurateY= self.AccurateY[2124:, :]
        self.AccurateYy= self.AccurateYy[2124:, :]

        self.Y = self.SeperateBy2Values(self.Y, 1, 28, Xval)
        self.Yy = self.SeperateBy2Values(self.Yy, 1, 12, YVal)
        #self.Y = self.SeperateBy2Values(self.Y, 0.5, 56, -8)
        #self.Yy = self.SeperateBy2Values(self.Yy, 0.5, 24, -4)

        self.NewY = np.array(self.Y, copy=True)
        self.NewYy = np.array(self.Yy, copy=True)
        self.CopyNewY = np.array(self.Y, copy=True)
        self.CopyNewYy = np.array(self.Yy, copy=True)
        self.NewY = np_utils.to_categorical(self.NewY, 28)
        self.NewYy = np_utils.to_categorical(self.NewYy, 12)
        #self.NewY = np_utils.to_categorical(self.NewY, 56)
        #self.NewYy = np_utils.to_categorical(self.NewYy, 24)
        #self.combineValues()
        self.differentProbabilities()
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


    def differentProbabilities(self):
        N = self.NewY
        P = self.NewYy

        for z,j in zip(N,P):
            i = np.where(z == 1)
            i = i[0]
            Values=np.array([i+1,i-1])
            Values1=np.array([i+2,i-2])
            Values2=np.array([i+3,i-3])
            UseBool = (Values < 28) & (Values >= 0)
            UseBool1 = (Values1 < 28) & (Values1 >= 0)
            UseBool2 = (Values2 < 28) & (Values2 >= 0)

            Values = Values[UseBool]
            Values1=Values1[UseBool1]
            Values2=Values2[UseBool2]

            #Values2 = Values2[UseBool2]

            length = len(Values)
            length1 = len(Values1)
            length2 = len(Values2)

            z[Values] = 0.2125
            z[Values1] = 0.075
            z[Values2] = 0.025
            z[i] = 0.35 + (2 - length)*0.2125+ (2 - length1)*0.075+ (2 - length2)*0.025

            i = np.where(j == 1)
            i = i[0]
            Values = np.array([i + 1, i - 1])
            Values1 = np.array([i + 2, i - 2])
            Values2 = np.array([i + 3, i - 3])
            UseBool = (Values < 12) & (Values >= 0)
            UseBool1 = (Values1 < 12) & (Values1 >= 0)
            UseBool2 = (Values2 < 12) & (Values2 >= 0)

            Values = Values[UseBool]
            Values1 = Values1[UseBool1]
            Values2 = Values2[UseBool2]

            # Values2 = Values2[UseBool2]

            length = len(Values)
            length1 = len(Values1)
            length2 = len(Values2)

            j[Values] = 0.2125
            j[Values1] = 0.075
            j[Values2] = 0.025
            j[i] = 0.35 + (2 - length) * 0.2125 + (2 - length1) * 0.075 + (2 - length2) * 0.025



        self.NewY = N



    def differentProbabilities4(self):
        N = self.NewY
        P = self.NewYy

        for z,j in zip(N,P):
            i = np.where(z == 1)
            i = i[0]
            Values=np.array([i+1,i-1])
            Values1=np.array([i+2,i-2])
            Values2=np.array([i+3,i-3])
            Values3=np.array([i+4,i-4])

            UseBool = (Values < 28) & (Values >= 0)
            UseBool1 = (Values1 < 28) & (Values1 >= 0)
            UseBool2 = (Values2 < 28) & (Values2 >= 0)
            UseBool3 = (Values3 < 28) & (Values3 >= 0)

            Values = Values[UseBool]
            Values1=Values1[UseBool1]
            Values2=Values2[UseBool2]
            Values3=Values3[UseBool3]

            #Values2 = Values2[UseBool2]

            length = len(Values)
            length1 = len(Values1)
            length2 = len(Values2)
            length3 = len(Values3)

            z[Values] = 0.2
            z[Values1] = 0.075
            z[Values2] = 0.025
            z[Values3] = 0.0125

            z[i] = 0.35 + (2 - length)*0.2125+ (2 - length1)*0.075+ (2 - length2)*0.025 + (2 - length3)*0.0125

            i = np.where(j == 1)
            i = i[0]
            Values = np.array([i + 1, i - 1])
            Values1 = np.array([i + 2, i - 2])
            Values2 = np.array([i + 3, i - 3])
            Values3 = np.array([i + 4, i - 4])

            UseBool = (Values < 12) & (Values >= 0)
            UseBool1 = (Values1 < 12) & (Values1 >= 0)
            UseBool2 = (Values2 < 12) & (Values2 >= 0)
            UseBool3 = (Values3 < 12) & (Values3 >= 0)

            Values = Values[UseBool]
            Values1 = Values1[UseBool1]
            Values2 = Values2[UseBool2]
            Values3 = Values3[UseBool3]

            # Values2 = Values2[UseBool2]

            length = len(Values)
            length1 = len(Values1)
            length2 = len(Values2)
            length3 = len(Values3)

            j[Values] = 0.2
            j[Values1] = 0.075
            j[Values2] = 0.025
            j[Values3] = 0.0125

            j[i] = 0.35 + (2 - length) * 0.2125 + (2 - length1) * 0.075 + (2 - length2) * 0.025 + (2 - length3) * 0.0125

        self.NewY = N




    def differentProbabilities5X5(self):
        N = self.NewY

        for z in N:
            i = np.where(z == 1)
            i = i[0]
            Values=np.array([i+1,i-1,i+13,i-13,i+11,i-11])
            Values2=np.array([i+2,i-2,i+14,i-14,i+10,i-10,i+22,i-22,i+26,i-26])
            Values3=np.array([i+25,i-25,i+23,i-23])
            Values4=np.array([i+24,i-24])

            Values1=np.array([i+12,i-12])

            # Values = np.array([i + 28, i - 28])
            # Values2 = np.array([i + 56, i - 56])
            #Values = np.array([i + 1, i - 1])
            #Values2 = np.array([i + 2, i - 2])
            UseBool = (Values < 336) & (Values >= 0) & (np.mod(Values,12) != 0) & (np.mod(Values,12)!=11)
            UseBool1 = (Values1 < 336) & (Values1 >= 0)
            UseBool2 = (Values2 < 336) & (Values2 >= 0)& (np.mod(Values2,12) != 0) & (np.mod(Values2,12)!=11)& (np.mod(Values2,12) != 1) & (np.mod(Values2,12)!=10)
            UseBool3 = (Values3 < 336) & (Values3 >= 0)& (np.mod(Values3,12) != 0) & (np.mod(Values3,12)!=11)
            UseBool4= (Values4 < 336) & (Values4 >= 0)


            #UseBool2 = (Values2 < 336) & (Values2 >= 0)

            Values = Values[UseBool]
            Values1=Values1[UseBool1]
            Values2=Values2[UseBool2]
            Values3=Values3[UseBool3]
            Values4=Values4[UseBool4]

            Values=np.append(Values,Values1)
            Values2=np.append(Values2,Values3)
            Values2=np.append(Values2,Values4)

            #Values2 = Values2[UseBool2]

            length = len(Values)
            length2 = len(Values2)

          #  length2 = len(Values2)

            z[Values] = 0.07
            z[Values2] = 0.02

            #z[Values2] = 0.05
            #z[i] = 0.7 + (2 - length) * 0.1 + (2 - length2) * 0.05
            z[i] = 0.12 + (8 - length)*0.07 + (16-length2)*0.02
        self.NewY = N




    def SeperateBy2Values(self,Y, byValue, uniqueValues, startingFrom):
        M1 = -100 + startingFrom
        M2 = +startingFrom
        Y2indices = (Y >= M1) & (Y <= M2)

        Y[Y2indices] = 1000
        ##########PROCESSING UX UY:)
        i = uniqueValues - 1
        z = 1001
        while i > 0:
            M1 = M2
            M2 += byValue
            if i == 1:
                Y2indices = (Y >= M1) & (Y <= M2 + 100)
            else:
                Y2indices = (Y >= M1) & (Y <= M2)
            Y[Y2indices] = z
            i -= 1
            z += 1

        Y -= 1000
        return Y

    def giveAModelNum(self):
        input1 = Input(shape=(9,), name='input1',)
        # input2=Input(shape=(84,),name='input2')

        # Together=concatenate([input1,input2])
        i = self.layers
        oneMoreLayer=(BatchNormalization())(input1)
        oneMoreLayer = Dense(self.LayerStandardWidth * (i + 1), activation='relu')(oneMoreLayer)
        if self.dropoutPercentage * i > 0.5:
            oneMoreLayer = Dropout(0.5)(oneMoreLayer)
        else:
            oneMoreLayer = Dropout(self.dropoutPercentage * i)(oneMoreLayer)
       # oneMoreLayer=(BatchNormalization())(oneMoreLayer)

        if self.NodesMultipliedByLayers ==True :
            while i > 1:
                oneMoreLayer = Dense(self.LayerStandardWidth * (i + 1), activation='relu')(oneMoreLayer)
              #  oneMoreLayer = (BatchNormalization())(oneMoreLayer)

                if self.dropoutPercentage * i > 0.5:
                    oneMoreLayer = Dropout(0.5)(oneMoreLayer)
                else:
                    oneMoreLayer = Dropout(self.dropoutPercentage * i)(oneMoreLayer)
                i -= 1
        else:
            while i > 1:
                oneMoreLayer = Dense(self.LayerStandardWidth  + (i*2), activation='relu')(oneMoreLayer)
                if self.dropoutPercentage * i > 0.5:
                    oneMoreLayer = Dropout(0.5)(oneMoreLayer)
                else:
                    oneMoreLayer = Dropout(self.dropoutPercentage * i)(oneMoreLayer)
                i -= 1

        outX = Dense(28, activation='softmax', name='outX')(oneMoreLayer)
        outY = Dense(12, activation='softmax', name='outY')(oneMoreLayer)

        model = Model(inputs=[input1], outputs=[outX, outY])

        model.compile(loss='categorical_crossentropy',
                      optimizer='adam',
                      metrics=['accuracy'])


        self.model=model

        plot_model(model, self.name+'Arch.png', True)
       # AngleModel.fit([X2[:2624, :]], [NewY[:2624, :]],
               #        epochs=1500, batch_size=2048, validation_split=0.05)


    def trainMeSenpai(self,epochs):
        #history = self.model.fit([self.X[:2124, :]], [self.NewY[:2124, :]],
           #                      epochs=epochs, batch_size=2048, validation_split=0.2)
        history=self.model.fit([self.X[:2124, :]], [self.NewY[:2124, :],self.NewYy[:2124, :]],
                  epochs=epochs, batch_size=2048,validation_split=0.2,verbose=0)

        #self.model.summary()
        #x=(history.history.keys())

       # print(history.history.keys())

        if hasattr(self,'History'):
            self.History.history['val_outX_acc'].extend(history.history['val_outX_acc'])
            self.History.history['val_outY_acc'].extend(history.history['val_outY_acc'])

            self.History.history[ 'loss'].extend(history.history['loss'])
            self.History.history['val_loss'].extend(history.history['val_loss'])
            self.History.history['outX_acc'].extend(history.history['outX_acc'])
            self.History.history['outY_acc'].extend(history.history['outY_acc'])

            history=self.History

        x = self.model.total_loss
       # print(history.history.keys())
        # summarize history for accuracy
        plt.clf()
        plt.plot(history.history['outX_acc'])
        plt.plot(history.history['outY_acc'])
        plt.plot(history.history['val_outX_acc'])
        plt.plot(history.history['val_outY_acc'])
        plt.text(0.5,0.5,str(np.max(history.history['val_outX_acc'])))
        plt.text(0.5,1,str(np.max(history.history['val_outY_acc'])))

        plt.title('model accuracy')
        plt.ylabel('accuracy')
        plt.xlabel('epoch')
        plt.legend(['trainX','trainY', 'testX' 'testY'], loc='upper left')
        plt.savefig(self.name+'Acc.png')
        #plt.show()
        # summarize history for loss
        plt.clf()
        plt.plot(history.history['loss'])
        plt.plot(history.history['val_loss'])
        plt.text(0.5,0.5,str(np.min(history.history['val_loss'])))
        plt.title('model loss')
        plt.yticks(np.arange(0,7,step=0.5))

        plt.ylabel('loss')
        plt.xlabel('epoch')
        plt.legend(['train', 'test'], loc='upper left')
        plt.savefig(self.name+'loss.png')
        self.History=history
        z=history.history['val_loss'].index(np.min(history.history['val_loss']))
        #self.model.fit([self.X[:2124, :]], [self.NewY[:2124, :]],
          #             epochs=z+1, batch_size=2048, validation_split=0.2,initial_epoch=z)
        self.LowestLossEpoch=z
        #######Kati gia epochs prp na ginei dame########
        #self.model.save(self.name+'Model.h5')


    def LoadMeSenpai(self):

        self.model = load_model(self.name+'Model.h5')
        '''
        history = self.model.fit([self.X[:2124, :]], [self.NewY[:2124, :]],
                                 epochs=500, batch_size=2048, validation_split=0.2,initial_epoch=99)
        self.model.summary()
        x = self.model.total_loss
        print(history.history.keys())
        # summarize history for accuracy
        plt.clf()
        plt.plot(history.history['acc'])
        plt.plot(history.history['val_acc'])
        plt.text(0.5,0.5,str(np.max(history.history['val_acc'])))
        plt.title('model accuracy')
        plt.ylabel('accuracy')
        plt.xlabel('epoch')
        plt.legend(['train', 'test'], loc='upper left')
        plt.savefig(self.name+'Acc.png')
        #plt.show()
        # summarize history for loss
        plt.clf()
        plt.plot(history.history['loss'])
        plt.plot(history.history['val_loss'])
        plt.text(0.5,0.5,str(np.min(history.history['val_loss'])))
        plt.title('model loss')
        plt.yticks(np.arange(0,7,step=0.5))

        plt.ylabel('loss')
        plt.xlabel('epoch')
        plt.legend(['train', 'test'], loc='upper left')
        plt.savefig(self.name+'loss.png')
        self.History=history
        #######Kati gia epochs prp na ginei dame########
        #self.model.save(self.name+'Model.h5')'''

    def predictArray(self, XVal, YVal,X):


        self.doThis(X)
        X=self.X
        arr = self.model.predict([self.X[:, :]], batch_size=8, verbose=1)

        class_result1 = arr[0]
        class_result2 = arr[1]
        class_result1 = np.reshape(class_result1, (int(class_result1.size / 28), 28))
        class_result2 = np.reshape(class_result2, (int(class_result2.size / 12), 12))

        class_result1 = np.argmax(class_result1, axis=1)
        class_result2 = np.argmax(class_result2, axis=1)
        class_result1 = np.reshape(class_result1, (class_result1.size, 1))

        class_result2 = np.reshape(class_result2, (class_result2.size, 1))

        class_result = np.append(class_result1, class_result2, 1)
        class_result3 = np.subtract(class_result1, XVal)
        class_result4 = np.subtract(class_result2, YVal)
        self.Result = np.append(class_result3, class_result4, 1)
        return  self.Result


    def predict(self,XVal,YVal):
        arr = self.model.predict([self.X[2124:, :]], batch_size=8, verbose=1)


        class_result1=arr[0]
        class_result2=arr[1]
        class_result1 = np.reshape(class_result1, (int(class_result1.size / 28), 28))
        class_result2 = np.reshape(class_result2, (int(class_result2.size / 12), 12))

        class_result1 = np.argmax(class_result1, axis=1)
        class_result2 = np.argmax(class_result2, axis=1)
        class_result1 = np.reshape(class_result1, (class_result1.size, 1))

        class_result2 = np.reshape(class_result2, (class_result2.size, 1))

        class_result = np.append(class_result1, class_result2, 1)
        class_result = np.append(class_result, self.CopyNewY[2124:, :], 1)
        class_result = np.append(class_result, self.CopyNewYy[2124:, :], 1)
        class_result3=np.subtract(class_result1,XVal)
        class_result4=np.subtract(class_result2,YVal)
        self.Result=np.append(class_result3, class_result4, 1)
       # self.Result[:,0]-=XVal
        #self.Result[:,1]-=YVal

        print(class_result)
        CopyY=self.CopyNewY[2124:, :]
        summed = np.power((class_result1 - CopyY), 2)
        summed = (np.sum(summed))
        print(summed)
        self.XTestLoss=summed/600

        CopyY=self.CopyNewYy[2124:, :]

        summed = np.power((class_result2 -  CopyY), 2)
        summed = (np.sum(summed))
        print(summed)

        self.YTestLoss=summed/600
        if hasattr(self,'allXlosses'):
                True
        else:
            self.allXlosses=[]
            self.allYlosses=[]

        self.allXlosses.append(self.XTestLoss)
        self.allYlosses.append(self.YTestLoss)

        print(class_result)
        if self.XTestLoss<0.49:
            return True
        return False


    def plots(self):
        Ylosses=np.array(self.allYlosses)
        Xlosses=np.array(self.allXlosses)
        plt.clf()
       # plt.plot(Ylosses)
        plt.bar(range(len(Ylosses)), Ylosses)

        plt.text(0.1, np.min(Ylosses), str(np.min(Ylosses)))
        plt.title(self.name+' YLossesPer 5Epoch')
        plt.ylabel('Loss')
        plt.xlabel('epoch')
        plt.savefig(self.name + 'YLosses.png')
        plt.clf()
        #plt.plot(Xlosses)
        plt.bar(range(len(Xlosses)), Xlosses)
        plt.text(0.1, np.min(Xlosses), str(np.min(Xlosses)))
        plt.title(self.name+'XLossesPer 5Epoch')
        plt.ylabel('Loss')
        plt.xlabel('epoch')
        plt.savefig(self.name + 'XLosses.png')







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
while numOfSeparation<8:
    layers = DefaultLayers
    Multi = True
    dropout = DefaultDropout  # 0 0.1 0.2 0.05 0.15 0.25
    width = DefaultWidth
    while layers<=7:
        while width <=32:
            while dropout <=0.021:
                Models.append(MyModel(X1, Y1, layers, features, Multi, width, dropout, epochs,-8-(7-numOfSeparation)*0.125,-4-(7-numOfSeparation)*0.125))
                #Models[i].trainMeSenpai(500)
                Models[i].LoadMeSenpai()

                i += 1
                dropout+=0.5
            use.append(i - 1)#etaraxa t topo
            dropout=DefaultDropout
            width+=1
        width=DefaultWidth
        layers+=1
    numOfSeparation+=1


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
        sock_connection(newsock,addr,Models)
        #thread = Thread(target=sock_connection, args=[newsock,addr,Models])
        #thread.start()




z=0
p=len(Models)
LossesX=[]
LossesY=[]

while z<p:
    plt.plot(Models[z].History.history['val_loss'])
    LossesX.append(Models[z].XTestLoss)
    LossesY.append(Models[z].YTestLoss)
    z+=1
plt.title('All Models model loss')
plt.ylabel('loss')
plt.xlabel('epoch')
plt.savefig('lossAllExtended.png')


LossesX=np.reshape(LossesX,[len(LossesX),1])
LossesY=np.reshape(LossesY,[len(LossesY),1])

plt.clf()
plt.bar(range(len(LossesX)),LossesX)

plt.title('All Models model TestXloss')
plt.ylabel('loss')
plt.xlabel('Model')
plt.savefig('lossAllXExtended.png')


plt.clf()
plt.bar(range(len(LossesY)),LossesY)

plt.title('All Models model TestYloss')
plt.ylabel('loss')
plt.xlabel('Model')
plt.savefig('lossAllYExtended.png')

