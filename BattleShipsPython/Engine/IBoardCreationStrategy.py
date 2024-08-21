from .GameSettings import GameSettings


class IBoardCreationStrategy:
    def create_board(self, settings: GameSettings) -> list:
        pass


class BoatPaster(IBoardCreationStrategy):
    def __init__(self, board):
        self.board = board

    def create_board(self, settings: GameSettings):
        return self.board
