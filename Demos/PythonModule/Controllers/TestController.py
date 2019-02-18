
from Controller import Controller

class TestController(Controller):
	def __init__(self):
		Controller.__init__(self)
		self.Register("User-5F674579-4D3F-42DE-A72C-A8B46AE94908",self.Test1)
		self.Register("User-1ECE00D8-614A-481F-861E-D20EEA55247C",self.Test2)
		self.Register("User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB",self.Test3)
	
	def Test1(self,num,str,peer,callback):
		print(num)
		print(str)

		self.Invoke("User-9CEF8CD0-8720-4C34-9341-545AF7693AB2",peer,None, str)
		return True

	def Test2(self,num,str,array,peer,callback):
		print(num)
		print(str)

		for	item in array:
			print(item)
		return True

	def Test3(self,num,str,array,packets,peer,callback):
		print(num)
		print(str)

		for	item in array:
			print(item)
		for item in packets:
			print(item.ToString())

		self.Invoke("User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C",peer,None, str,packets[0])
		return True