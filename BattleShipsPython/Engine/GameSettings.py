def default():
    return GameSettings(10, 10, [4, 3, 2, 1])


class GameSettings:
    height: int
    width: int
    ship_count: list

    def __init__(self, width, height, ship_count):
        self.width = width
        self.height = height
        self.ship_count = ship_count
