from Engine.IGameStrategy import IGameStrategy
from Engine.GameSettings import GameSettings
from random import randint


class DefaultGameStrategy(IGameStrategy):
    settings: GameSettings

    def start(self, settings):
        self.settings = settings

    def move(self):
        return (randint(0, self.settings.width - 1), randint(0, self.settings.height - 1))
