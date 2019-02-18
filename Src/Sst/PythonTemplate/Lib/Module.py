
import clr
import sys
clr.AddReference("Sss")

from Sss.SssScripts.Python import PythonProxy

class Module:
	def __init__(self):
		self.IModule = PythonProxy.CreateModule(self,sys.PythonHelper)
		self.ServiceId = ""

	def Initialize(self,server,cacheManager,controllerComponentManager):
		pass
	
	def InitFinish(self,server,cacheManager,controllerComponentManager):
		pass

	def Finish(self,server,cacheManager,controllerComponentManager):
		pass

	def Dispose(self,cacheManager,controllerComponentManager):
		pass

	def Accepted(self,peer,readStream,writeStream):
		return True

	def Connected(self,peer,readStream):
		pass

	def Disconnected(self,peer):
		pass

	def AddController(self,generator):
		return self.IModule.AddController(generator)

	def AddPacket(self,interface,implement):
		self.IModule.AddPacket(interface,implement)