import random
from Engine.GameSettings import GameSettings
from Engine.IGameStrategy import IGameStrategy


class ChatGPTGameStrategy(IGameStrategy):
    def __init__(self):
        self.board = []
        self.possible_moves = []
        self.hits = []
        self.target_mode = False
        self.last_move = None

    def start(self, settings: GameSettings):
        # Initialize an empty board
        self.board = [['~' for _ in range(settings.width)] for _ in range(settings.height)]

        # Initialize the list of all possible moves
        self.possible_moves = [(r, c) for r in range(settings.height) for c in range(settings.width)]
        random.shuffle(self.possible_moves)  # Shuffle to ensure randomness

    def move(self) -> (int, int):
        if self.target_mode and self.hits:
            # Target mode: try to sink the ship
            row, col = self._target_ship()
        else:
            # Random hunt mode: pick a random move from the possible moves
            row, col = self.possible_moves.pop()
            self.last_move = (row, col)

        return row, col

    def hit(self):
        # Mark the hit on the board
        row, col = self.last_move
        self.board[row][col] = 'H'
        self.hits.append((row, col))

        # Enter target mode to find and sink the ship
        self.target_mode = True

    def miss(self):
        # Mark the miss on the board
        row, col = self.last_move
        self.board[row][col] = 'M'

        # If we're in target mode, continue to try to find the ship
        if self.target_mode and not self.hits:
            self.target_mode = False

    def sunk(self):
        # Clear hits since the ship is sunk
        self.hits = []
        self.target_mode = False

    def _target_ship(self) -> (int, int):
        """Find the next logical move to try and sink a ship based on the current hits."""
        potential_targets = []

        for hit in self.hits:
            row, col = hit
            # Check surrounding cells (up, down, left, right)
            for dr, dc in [(-1, 0), (1, 0), (0, -1), (0, 1)]:
                r, c = row + dr, col + dc
                if 0 <= r < len(self.board) and 0 <= c < len(self.board[0]) and self.board[r][c] == '~':
                    potential_targets.append((r, c))

        if potential_targets:
            target = random.choice(potential_targets)
            self.possible_moves.remove(target)  # Remove from possible moves
            self.last_move = target
            return target

        # If no valid targets, revert to hunt mode
        self.target_mode = False
        return self.move()  # Call move again in hunt mode
