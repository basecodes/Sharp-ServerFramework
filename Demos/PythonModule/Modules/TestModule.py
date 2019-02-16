from Module import Module
from TestController import TestController
from ITestPacket import ITestPacket
from TestPacket import TestPacket

class TestModule(Module):
	def __init__(self):
		self.ServiceId = "Test"

	def Initialize(self,server,cacheManager,controllerComponentManager):
		Module.Initialize(self,server,cacheManager,controllerComponentManager)
		self.AddController(lambda :TestController())
		imp = TestPacket()
		print(imp.TypeName)
		self.AddPacket(ITestPacket,lambda :TestPacket())