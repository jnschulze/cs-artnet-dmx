all:
	mcs test.cs artnet.cs -unsafe -out:dmx_test.exe

clean:
	rm dmx_test.exe
