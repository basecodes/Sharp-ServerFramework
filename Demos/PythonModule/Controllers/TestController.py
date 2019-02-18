
from Controller import Controller

class TestController(Controller):
	def __init__(self):
		Controller.__init__(self)
		self.Register("User-5F674579-4D3F-42DE-A72C-A8B46AE94908",self.Test1)
		self.Register("User-1ECE00D8-614A-481F-861E-D20EEA55247C",self.Test2)
		self.Register("User-26D0A8C7-3D9B-4AC9-B6AF-700A61E23BFB",self.Test3)
		self.Register("User-4844F488-2169-4AAE-A93B-56E45E10495B",self.Test4)
	
	def Test1(self,num,str,peer,callback):
		print(num)
		print(str)

		self.Invoke("User-9CEF8CD0-8720-4C34-9341-545AF7693AB2",peer,None, num,str)
		return True

	def Test2(self,num,str,array,peer,callback):
		print(num)
		print(str)

		for	item in array:
			print(item)

		self.Invoke("User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C",peer,None, num,str,array)
		return True

	def Test3(self,num,str,array,packets,peer,callback):
		print(num)
		print(str)

		for	item in array:
			print(item)
		for item in packets:
			print(item.ToString())

		self.Invoke("User-444E0735-DA0B-4A29-9746-E7FEFE7E2293",peer,None,num,str,array,packets)
		return True

	def Test4(self,str,dict,peer,callback):
		print(str)

		for item in dict:
			print(item)
			print(dict[item])

		self.Invoke("User-C00E44A2-09E0-4C94-81DC-9622AA38EFB4",peer,None,str,dict)
		return True