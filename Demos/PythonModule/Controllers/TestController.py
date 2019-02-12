
from Controller import Controller

class TestController(Controller):
	def __init__(self):
		Controller.__init__(self)
		self.Register("+[User-5F674579-4D3F-42DE-A72C-A8B46AE94908]+",self.Test1)
		self.Register("+[User-1ECE00D8-614A-481F-861E-D20EEA55247C]+",self.Test2)
		self.Register("+[User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB]+",self.Test3)
	
	def Test1(self,num,str,peer,callback):
		print(num)
		print(str)
		self.Invoke("User-9CEF8CD0-8720-4C34-9341-545AF7693AB2",peer,None, str)
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