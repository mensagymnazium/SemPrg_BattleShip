import socket
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from PIL import Image

# Change if needed.
HOST = "127.0.1.1"
PORT = 65431

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

browser = webdriver.Chrome()
browser.set_window_size(1200,800)
browser.get("http://en.battleship-game.org/")
browser.save_screenshot("page.png")
browser.quit()

string = ""

image = Image.open("page.png")
image_loaded = image.load()
for i in range(10):
    for j in range(10):
        pixel = image_loaded[180+33*i,167+33*j]
        if pixel[0] < 249:
            string += str(i) + "," + str(j) + "+"
string = string[:-1] + "<EOF>"
print(string)

while True:
    data = str(s.recv(1024))
    print(data)
    if data.find("Turn off") > -1:
        s.close()
        break
    if data.find("Send") > -1:
        s.sendall(bytes(string, "UTF-8"))

