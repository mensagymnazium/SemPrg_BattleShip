from .Game import Game


class Participant:
    def __init__(self, name, game_strategy, board_strategy):
        self.name = name
        self.game_strategy = game_strategy
        self.board_strategy = board_strategy


class Tournament:
    def __init__(self, participants, boards_per_round):
        self.participants = participants
        self.boards_per_round = boards_per_round

    def play(self, settings):
        results = {}
        for participant in range(len(self.participants)):
            results[participant] = 0
        for participant in range(len(self.participants)):
            print("\nBoards of " + str(self.participants[participant].name))
            current_scores = {}
            for i in range(len(self.participants)):
                if i != participant:
                    current_scores[i] = 0

            for j in range(self.boards_per_round):
                game = Game(settings, self.participants[participant].board_strategy)
                for competitor in range(len(self.participants)):
                    if participant != competitor:
                        result = game.simulate_game(self.participants[competitor].game_strategy)
                        results[competitor] += result
                        current_scores[competitor] += result

            for i in range(len(self.participants)):
                if i != participant:
                    print(self.participants[i].name + ": " + str(current_scores[i]))
        self.print_results(results)

    def print_results(self, results):
        print("\nResults:")
        sorted_results = dict(sorted(results.items(), key=lambda item: item[1]))
        for participant in sorted_results.keys():
            print(self.participants[participant].name + ": " + str(sorted_results[participant]) + " turns ("
                  + str(sorted_results[participant] / (len(self.participants)-1) / self.boards_per_round)
                  + " on average)")
