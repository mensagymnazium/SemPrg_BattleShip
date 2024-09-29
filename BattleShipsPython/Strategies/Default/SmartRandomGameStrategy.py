from Engine.GameSettings import GameSettings
from Engine.IGameStrategy import IGameStrategy
from random import randint


class SmartRandomGameStrategy(IGameStrategy):
    settings: GameSettings
    shots: list

    def start(self, settings):
        self.settings = settings
        self.shots = []

    def move(self):
        while True:
            shot = (randint(0, self.settings.width - 1), randint(0, self.settings.height - 1))
            if shot not in self.shots:
                self.shots.append(shot)
                return shot
