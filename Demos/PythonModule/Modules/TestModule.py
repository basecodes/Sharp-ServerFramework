from Module import Module
from ITestController import ITestController
from TestController import TestController
from ITestPacket import ITestPacket
from TestPacket import TestPacket

class TestModule(Module):
	def __init__(self):
		self.ServiceId = "Test"

	def Initialize(self,server,cacheManager,controllerComponentManager):
		super(Module,self).Initialize(server,cacheManager,controllerComponentManager)
		self.AddController(ITestController,TestController())
		self.AddPacket(ITestPacket,lambda :TestPacket())