local classes = require "Classes"

local ITestRequest = classes.class()

function ITestRequest:init(child)
	self.super:init(child)

	-- 格式 +[id]+
    self["+[User-9CEF8CD0-8720-4C34-9341-545AF7693AB2]+"] = child.Test1
	self["+[User-4AC85EE0-2616-4EB3-AD50-DA7FB588870C]+"] = child.Test2
end

function ITestRequest:Test1(str)
	return true
end

function ITestRequest:Test2(str,packet)
	return true
end

return ITestRequest