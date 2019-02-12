
import clr
import sys
clr.AddReference("Sss")

from Sss.SssModule import PythonModule

class Module(PythonModule):
	def __init__(self):
		PythonModule.SetPythonHelper(self,sys.PythonHelper)
		self.__ServiceId = ""

	@property
	def ServiceId(self):
		return self.__ServiceId

	@ServiceId.setter
	def ServiceId(self,value):
		self.__ServiceId = value

	def Initialize(self,server,cacheManager,controllerComponentManager):
		PythonModule.Initialize(self,server,cacheManager,controllerComponentManager)
	
	def InitFinish(self,server,cacheManager,controllerComponentManager):
		PythonModule.InitFinish(self,server,cacheManager,controllerComponentManager)

	def Finish(self,server,cacheManager,controllerComponentManager):
		PythonModule.Finish(self,server,cacheManager,controllerComponentManager)

	def Dispose(self,cacheManager,controllerComponentManager):
		PythonModule.Dispose(self,cacheManager,controllerComponentManager)

	def Accepted(self,peer,readStream,writeStream):
		return PythonModule.Accepted(self,peer,readStream,writeStream)

	def Connected(self,peer,readStream):
		PythonModule.Connected(self,peer,readStream)

	def AddController(self,generator):
		return PythonModule.AddController(self,generator)

	def SetObjectPool(self,interface,implement):
		PythonModule.SetObjectPool(self,interface,implement)

	def SetPacketPool(self,interface,implement):
		PythonModule.SetPacketPool(self,interface,implement)

	def AddPacket(self,interface,implement):
		PythonModule.AddPacket(self,interface,implement)