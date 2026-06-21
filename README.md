# AI Parking — Reinforcement Learning (Unity ML-Agents)

![Unity](https://img.shields.io/badge/Unity-000000?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![ML-Agents](https://img.shields.io/badge/ML--Agents-0098A6?logo=unity&logoColor=white)

A **reinforcement-learning** project built with Unity **ML-Agents**: an autonomous car agent learns to park itself in a simulated environment through trial-and-error and reward shaping.

> **Stack:** Unity · C# · Unity ML-Agents · Prometeo Car Controller

## Overview

The agent (`Assets/Scripts/CarAgent.cs`) is trained with Unity's ML-Agents framework. It observes its surroundings, controls a realistic vehicle (via the Prometeo car controller), and is rewarded for navigating into a target parking spot without collisions. Randomised episodes (`ShuffleScript.cs`) vary the starting conditions so the policy generalises.

## How it works

- **Agent** — `CarAgent.cs` defines observations, actions (steering / throttle), and the reward function
- **Vehicle physics** — realistic driving via the Prometeo Car Controller
- **Training** — Unity ML-Agents (PPO) drives the learning loop
- **Episode variation** — `ShuffleScript.cs` randomises spawn/parking positions for robustness

## Getting Started

```bash
# 1. Open the project in Unity (matching the project's Editor version)
# 2. Install the ML-Agents package (Package Manager) + Python mlagents:
pip install mlagents

# 3. Train:
mlagents-learn config/parking.yaml --run-id=parking_01
# then press Play in the Unity Editor to start the simulation
```

## Demo

<!-- A training/parking clip lives in your portfolio — embed or link the GIF here. -->

---

Built by [Salah Bounouh](https://github.com/salahbnh) · [Portfolio](https://salahbounouh.com) · [LinkedIn](https://www.linkedin.com/in/salah-bounouh-1426ba27b/)
