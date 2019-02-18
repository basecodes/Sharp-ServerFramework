import clr
import sys

clr.AddReference("Sss")

from Sss.SssScripts.Python import PythonProxy

class Controller:
	def __init__(self):
		self.MethodIds = ""
		self.IController = PythonProxy.CreateControler(self)

	def Register(self, key,value):
		id = self.IController.Register(key,value,sys.PythonHelper)
		if self.MethodIds == "":
			self.MethodIds = id
		else:
			self.MethodIds = self.MethodIds + ";" + id

	def Invoke(self,id,peer,callback,*args):
		self.IController.Invoke(id,peer,callback,args)