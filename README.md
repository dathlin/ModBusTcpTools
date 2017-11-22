# ModBusTcpTools
一个Modbus Tcp的C#开发示例，运用HslCommunication.dll组件库提供的功能类实现。

关于该组件库的详细API博客见
[http://www.cnblogs.com/dathlin/p/7703805.html](http://www.cnblogs.com/dathlin/p/7703805.html)
目前版本号：[![NuGet Status](https://img.shields.io/nuget/v/HslCommunication.svg)](https://www.nuget.org/packages/HslCommunication/)

NuGet 安装方式：

Install-Package HslCommunication

## Server Side
允许输入一个端口号，方便的创建服务器程序，接收来自所有的客户端发过来的Modbus tcp数据，并触发信息，显示在下面的编辑框里。暂时不支持手动控制返回数据，如果有需求，后面的更新版本再追加。如下就是接收到一个客户端发来数据：

![](https://github.com/dathlin/ModBusTcpTools/raw/master/image/server1.png)

## Client Side
客户端的实现，可以方便的用来测试和服务器端的数据通讯，允许收发基础数据，也可以调用一些扩展的API，具体代码可以参照本项目，也可以参考博客。下面的客户端连接了西门子PLC的ModBus Tcp服务器。

* 第一种情况演示了输入完整的数据指令，然后会请求服务器返回对应的数据。

![](https://github.com/dathlin/ModBusTcpTools/raw/master/image/client1.png)

* 如下的界面演示了读取线圈时候的操作。

![](https://github.com/dathlin/ModBusTcpTools/raw/master/image/client2.png)

* 如下的界面在地址为6的数据上写入1234这个数据，然后在编辑框里显示写入成功。

![](https://github.com/dathlin/ModBusTcpTools/raw/master/image/client3.png)

* 我们看看上述的写入是否真的成功了，重新读取服务器的数据，这次用指令的方式，返回04D2，十进制刚好1234，所以写入成功了。

![](https://github.com/dathlin/ModBusTcpTools/raw/master/image/client4.png)