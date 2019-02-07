import clr
clr.AddReference("Ssm")

from Ssm.Ssm import Controller as PythonController

class Controller(PythonController):
	def __init__(self):
		self.__dict = dict()
		
	def Members(self):
		return self.__dict;

	def __setitem__(self, key,value):
		self.__dict[key] = value
		
	def __getitem__(self, key):
		return self.__dict[key]