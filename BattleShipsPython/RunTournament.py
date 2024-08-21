from Engine import GameSettings
from Engine.SimpleTournament import Tournament, Participant
from Strategies.Default.DefaultGameStrategy import DefaultGameStrategy
from Strategies.Default.DefaultBoardCreationStrategy import DefaultBoardCreationStrategy
from Strategies.Default.SmartRandomGameStrategy import SmartRandomGameStrategy
from Strategies.ChatGPT.ChatGPTBoardCreationStrategy import ChatGPTBoardCreationStrategy
from Strategies.ChatGPT.ChatGPTGameStrategy import ChatGPTGameStrategy


participants = [
    Participant("Default", DefaultGameStrategy(), DefaultBoardCreationStrategy()),
    Participant("SmartRandom", SmartRandomGameStrategy(), ChatGPTBoardCreationStrategy()),
    Participant("ChatGPT", ChatGPTGameStrategy(), ChatGPTBoardCreationStrategy())
]
tournament = Tournament(participants, 100)
tournament.play(GameSettings.default())
