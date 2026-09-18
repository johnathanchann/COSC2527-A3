## COSC2527-A3
A Unity project developed for **COSC2527 Games and Artificial Intelligence Techniques** at RMIT University, demonstrating rule-based and learning-based game AI in a two-dimensional soccer setting. Two AI-controlled players compete head to head to score goals.

## Algorithms
- **Behaviour Tree**: Priority-based selector/sequence tree covering attacking, defending and ball possession
- **Reinforcement Learning**: Proximal Policy Optimization (PPO) via Unity ML-Agents, rewarded for scoring and penalised for conceding
- **Self-Play**: Training against past snapshots of the agent's own policy, with Elo-rated opponent sampling
- **Curriculum Learning**: Staged difficulty, progressing from an unopposed scenario to full matches
- **Multi-Agent Learning**: 2v2 teams trained with a shared group reward
- **Steering**: Acceleration-limited movement towards a desired velocity
- **Ball Physics**: Magnus effect for curved kicks

## Manual Controls
- **WASD / Arrow keys**: Move the player
- **Space**: Straight kick
- **Q**: Curved kick to the left
- **E**: Curved kick to the right

---
## Game Objective
- Kick the ball into the opponent's goal
- Score more goals than the opponent

## Game Mechanics
- **Movement**: Players accelerate towards their chosen direction rather than changing velocity instantly, so movement carries momentum.
- **Kicking**: A player can kick only when the ball is within reach. Curved kicks put spin on the ball, and the Magnus effect bends its path.
- **Scoring**: The scoreboard updates after each goal, and the players and ball return to their starting positions.
- **Obstacles**: Obstacles can optionally be placed at random positions on the field.
- **AI players**: Either side can be controlled by the behaviour tree agent or a trained reinforcement learning model.

---
## Running the Game
- Open the **Arena** scene
- Enable any parent object

## Training Agents
- Open **NathanTrainingEnv**, **DanTrainingEnv** or **JohnathanTraining Env**
- Run a config file from `config/`

## Models
- `Assets/TFModels/Nathan`: Nathan's models
- `Assets/TFModels/Johnathan`: Johnathan's models
- `Assets/TFModels/Dan`: Dan's models

**Note**: The starter project was created in Unity 6000.0.37f1.
