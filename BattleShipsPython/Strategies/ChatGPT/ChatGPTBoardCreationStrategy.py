import random
from Engine.IBoardCreationStrategy import IBoardCreationStrategy
from Engine.GameSettings import GameSettings


class ChatGPTBoardCreationStrategy(IBoardCreationStrategy):
    def create_board(self, settings: GameSettings) -> list:
        while True:
            success = True  # Added this logic manually to prevent problems

            # Create an empty board
            board = [[0 for _ in range(settings.width)] for _ in range(settings.height)]

            # Ship placement
            for ship_length, count in enumerate(settings.ship_count, start=1):
                for _ in range(count):
                    success = self.place_ship(board, ship_length, settings)
                    if not success:
                        break
                if not success:
                    break

            if success:
                return board

    def place_ship(self, board: list, ship_length: int, settings: GameSettings):
        unsuccessful = 0
        placed = False

        while not placed:
            # Randomly choose the orientation of the ship
            orientation = random.choice(['horizontal', 'vertical'])

            if orientation == 'horizontal':
                row = random.randint(0, settings.height - 1)
                col = random.randint(0, settings.width - ship_length)

                # Check if space is free and no collision on edges/sides
                if self.can_place_ship(board, row, col, ship_length, orientation):
                    # Place the ship
                    for i in range(ship_length):
                        board[row][col + i] = 1
                    placed = True
                else:
                    unsuccessful += 1
            else:  # vertical
                row = random.randint(0, settings.height - ship_length)
                col = random.randint(0, settings.width - 1)

                # Check if space is free and no collision on edges/sides
                if self.can_place_ship(board, row, col, ship_length, orientation):
                    # Place the ship
                    for i in range(ship_length):
                        board[row + i][col] = 1
                    placed = True
                else:
                    unsuccessful += 1
            if unsuccessful > 300:
                return False
        return True

    def can_place_ship(self, board: list, row: int, col: int, ship_length: int, orientation: str) -> bool:
        """Check if the ship can be placed on the board without collisions, including edges and sides."""

        # Determine the range of rows and columns to check based on the orientation
        if orientation == 'horizontal':
            row_start = max(0, row - 1)
            row_end = min(len(board), row + 2)
            col_start = max(0, col - 1)
            col_end = min(len(board[0]), col + ship_length + 1)
        else:  # vertical
            row_start = max(0, row - 1)
            row_end = min(len(board), row + ship_length + 1)
            col_start = max(0, col - 1)
            col_end = min(len(board[0]), col + 2)

        # Check the area around the ship for any existing ships
        for r in range(row_start, row_end):
            for c in range(col_start, col_end):
                if board[r][c] == 1:
                    return False

        return True
