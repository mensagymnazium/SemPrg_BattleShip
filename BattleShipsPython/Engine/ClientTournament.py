from .SimpleTournament import Participant, Tournament
from .IBoardCreationStrategy import BoatPaster
from .Game import Game
import socket


class ClientTournament(Tournament):
    def __init__(self, participants, boards_per_round, host, port):
        super().__init__(participants, boards_per_round)
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.connect((host, port))
        self.scores = {}

    def play(self, settings):
        for i in range(len(self.participants)):
            self.scores[i] = 0

        while True:
            command = self.socket.recv(1024).decode()
            command = command.replace("<EOM>", "")

            if command == "Close!":
                self.socket.close()
                self.print_results(self.scores)
                return

            elif command == "Send your results.":
                response = ""
                for i in range(len(self.participants)):
                    response += self.participants[i].name + ":" + str(self.scores[i])
                    if i != len(self.participants) - 1:
                        response += ","
                response += "<EOM>"
                self.socket.sendall(bytes(response, "UTF-8"))

            elif command == "Play a game.":
                self.socket.sendall(bytes("Ready.<EOM>", "UTF-8"))
                board = [[0 for _ in range(settings.width)] for _ in range(settings.height)]

                ships = ""
                while True:
                    ships = self.socket.recv(1024).decode()
                    if ships.find("<EOM>") > -1:
                        ships = ships.replace("<EOM>", "")
                        break
                ships = ships.split("+")
                for ship in ships:
                    parts = [int(i) for i in ship.split(",")]
                    board[parts[0]][parts[1]] = 1

                game = Game(settings, BoatPaster(board))
                for competitor in range(len(self.participants)):
                    self.scores[competitor] += game.simulate_game(self.participants[competitor].game_strategy)

                self.socket.sendall(bytes("Done.<EOM>", "UTF-8"))

            elif command == "Settings.":
                self.socket.sendall(bytes("Ready.<EOM>", "UTF-8"))
                while True:
                    data = self.socket.recv(1024).decode()
                    if data.find("<EOM>") > -1:
                        break
                # It was easier to just add the settings as a parameter for now.
                self.socket.sendall(bytes("Done.<EOM>", "UTF-8"))

            elif command == "Lead.":
                self.lead(settings)
                self.socket.sendall(bytes("Done.<EOM>", "UTF-8"))

    def lead(self, settings):
        local_scores = {}
        for i in range(len(self.participants)):
            local_scores[i] = 0

        for participant in range(len(self.participants)):
            print("\nBoards of " + str(self.participants[participant].name))
            current_scores = {}
            for i in range(len(self.participants)):
                if i != participant:
                    current_scores[i] = 0

            for j in range(self.boards_per_round):
                board = self.participants[participant].board_strategy.create_board(settings)
                ships = ""
                for column in range(settings.width):
                    for row in range(settings.height):
                        if board[column][row] == 1:
                            ships += str(column) + "," + str(row) + "+"
                ships = ships[:-1] + "<EOM>"
                self.socket.sendall(bytes(ships, "UTF-8"))

                game = Game(settings, BoatPaster(board))
                for competitor in range(len(self.participants)):
                    if participant != competitor:
                        result = game.simulate_game(self.participants[competitor].game_strategy)
                        self.scores[competitor] += result
                        local_scores[competitor] += result
                        current_scores[competitor] += result

                while True:
                    data = self.socket.recv(1024).decode()
                    if data.find("<EOM>") > -1:
                        break

            for i in range(len(self.participants)):
                if i != participant:
                    print(self.participants[i].name + ": " + str(current_scores[i]))

        self.print_results(local_scores)
