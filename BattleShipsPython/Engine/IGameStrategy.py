from .GameSettings import GameSettings


class IGameStrategy:
    def start(self, settings: GameSettings):
        pass

    def move(self) -> (int, int):
        pass

    def hit(self):
        pass

    def miss(self):
        pass

    def sunk(self):
        pass
