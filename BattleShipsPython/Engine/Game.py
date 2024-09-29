from .GameSettings import GameSettings
from .IGameStrategy import IGameStrategy
from .IBoardCreationStrategy import IBoardCreationStrategy


def ship_is_dead(board, place, direction, settings):
    if board[place[0]][place[1]] == 1:
        return False
    if board[place[0]][place[1]] == 0:
        return True

    result = True
    if direction in [0, 4] and place[0] > 0:
        new_place = (place[0] - 1, place[1])
        result = result and ship_is_dead(board, new_place, 0, settings)
    if direction in [1, 4] and place[1] > 0:
        new_place = (place[0], place[1] - 1)
        result = result and ship_is_dead(board, new_place, 1, settings)
    if direction in [2, 4] and settings.width - 1 > place[0]:
        new_place = (place[0] + 1, place[1])
        result = result and ship_is_dead(board, new_place, 2, settings)
    if direction in [3, 4] and settings.height - 1 > place[1]:
        new_place = (place[0], place[1] + 1)
        result = result and ship_is_dead(board, new_place, 3, settings)
    return result


def all_ships_are_dead(board):
    for column in board:
        for tile in column:
            if tile == 1:
                return False
    return True


class Game:
    def __init__(self, settings: GameSettings, board_strategy: IBoardCreationStrategy):
        self.settings = settings
        self.board = board_strategy.create_board(settings)  # Expect the board to be valid

    #    def __init__(self, settings, board):
    #        self.settings = settings
    #        self.board = board  # Expect the board to be valid

    def simulate_game(self, strategy: IGameStrategy):
        board = []
        for column in self.board:
            board.append(column.copy())
        strategy.start(self.settings)
        i = 0
        for i in range(1, self.settings.width * self.settings.height * 5 + 1):
            move = strategy.move()
            if board[move[0]][move[1]] == 1:
                strategy.hit()
                board[move[0]][move[1]] = 2
                if ship_is_dead(board, move, 4, self.settings):
                    if all_ships_are_dead(board):
                        break
                    strategy.sunk()
            else:
                strategy.miss()
        return i  # The number of turns
