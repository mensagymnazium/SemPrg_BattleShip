from Engine.IBoardCreationStrategy import IBoardCreationStrategy


class DefaultBoardCreationStrategy(IBoardCreationStrategy):
    def create_board(self, settings):
        board = []
        for i in range(settings.width):
            column = []
            for j in range(settings.height):
                column.append(0)
            board.append(column)

        active_column = 0
        place = 0
        for ship_length in range(1, len(settings.ship_count) + 1):
            for ship in range(settings.ship_count[ship_length-1]):
                if place + ship_length <= settings.height:
                    for tile in range(ship_length):
                        board[active_column][place + tile] = 1
                    place += ship_length + 1
                else:
                    active_column += 2
                    for tile in range(ship_length):
                        board[active_column][tile] = 1
                    place = ship_length + 1
        return board
