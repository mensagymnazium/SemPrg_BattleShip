import socket
from playwright.sync_api import sync_playwright
from PIL import Image

# Change if needed.
HOST = "127.0.1.1"
PORT = 65431

s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
s.connect((HOST, PORT))

playwright = sync_playwright().start()
browser = playwright.chromium.launch()
page = browser.new_page()
page.set_viewport_size({"width": 1200, "height": 800})
page.goto("http://en.battleship-game.org")
page.screenshot(path="page.png")
browser.close()

string = ""

image = Image.open("page.png")
image_loaded = image.load()
for i in range(10):
    for j in range(10):
        pixel = image_loaded[205+33*i,167+33*j]
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

