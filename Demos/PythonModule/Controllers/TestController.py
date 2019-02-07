
from ITestController import ITestController

class TestController(ITestController):
	def __init__(self):
		ITestController.__init__(self)
	
	def Test1(self,num,str,peer,callback):
		print(num)
		print(str)
		return True

	def Test2(self,num,str,array,peer,callback):
		self.Test1(num,str,peer,callback)
		for	item in array:
			print(item)
		return True

	def Test3(self,num,str,array,packet,peer,callback):
		self.Test2(num,str,array,peer,callback)
		for item in packet:
			print(item.ToString())
		return True