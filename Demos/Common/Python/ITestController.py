
from Controller import Controller

class ITestController(Controller):
	def __init__(self):
		Controller.__init__(self)

		# 格式 +[id]+
		self["+[User-5F674579-4D3F-42DE-A72C-A8B46AE94908]+"] = self.Test1
		self["+[User-1ECE00D8-614A-481F-861E-D20EEA55247C]+"] = self.Test2
		self["+[User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB]+"] = self.Test3
	
	def Test1(self,num,str,peer,callback):
		pass

	def Test2(self,num,str,array,peer,callback):
		pass

	def Test3(self,num,str,array,packet,peer,callback):
		pass
