# Balloon Pop — XR Serious Game for Fine Motor Training in Parkinson's Disease

A mixed-reality serious game developed in Unity for the **Meta Quest 3S**, designed to assessand train **fine motor skills** in people with early-stage Parkinson's disease. The core interaction is built on the **finger-tapping** gesture, the clinical reference for evaluating bradykinesia.
This repository hosts the Unity project and the full thesis document produced for the M.Sc. in Biomedical Engineering (LM-21) at the University of Naples Federico II.

## Overview

Parkinson's disease (PD) is the second most common neurodegenerative disorder worldwide and the fastest-growing neurological condition. Among its cardinal symptoms, bradykinesia is both the main diagnostic criterion and one of the most disabling, progressively impairing fine motor control and the patient's autonomy in daily activities. Extended reality (XR) and serious games are emerging as promising tools to support clinical assessment and motor–cognitive rehabilitation.

This work was carried out within a broader project supporting the assessment and rehabilitation of patients with neurological conditions, in collaboration with the **Neurology Department of the A.O.U. Federico II of Naples**, which provided the clinical input and helped define the evaluation criteria, and with **WipLab Srl**, a software house specialised in extended-reality applications.

The thesis describes the development of the serious game **Balloon Pop**. Its central contribution is the **porting of the application from Microsoft HoloLens 2 to the Meta Quest 3S**. This migration is motivated by clinical accessibility: the Meta headset is markedly cheaper and far more widespread, concretely lowering the adoption barrier for rehabilitation centres and opening the door to home use.

## The game

In Balloon Pop the patient sits in a passthrough mixed-reality scene where virtual balloons rise in
front of them. A ray cast from the hand intercepts a balloon, which is then popped through a
finger-tapping (pinch) gesture. The application runs in two modes.

### Single-player

Five levels of increasing complexity, each played with both hands and each targeting a specific
clinical objective:

| Level | Mode | Goal | Skills assessed |
|-------|------|------|-----------------|
| 0 | Training (~1 min) | Familiarisation with the system | — |
| 1 | Baseline (~2:30) | Pop as many balloons as possible | Finger-tapping execution |
| 2 | Cognitive dual-task (~2:30) | Pop only the yellow balloons | Selective attention, inhibitory control |
| 3 | Motor dual-task (~2:30) | Pop balloons + contralateral tapping | Bimanual coordination |
| 4 | Cognitive dual-task (~2:30) | Finger-tapping + spoken subtractions | Executive function |

At the end of a session the system records the level name, the hand used, and the number of balloons
hit and missed. Data are serialised to **JSON**, stored in the headset's internal memory, and sent
automatically to the clinician by e-mail over **SMTP**.

### Multiplayer (cooperative)

A cooperative two-player level intended to stimulate social interaction and counter the isolation
associated with disease progression. Each player has colour-coded targets (Player 1: red, Player 2:
yellow) plus a shared blue target, on three synchronised balloon columns. The architecture is
**client–server with the clinician's PC acting as the authoritative server**: the headsets discover
the server automatically over the LAN, hit validation runs server-side, and game state is propagated
to all devices. Session data are saved locally on the PC as JSON and can be browsed from a dedicated
panel.

## Tech stack

- **Engine:** Unity 6 LTS
- **XR SDK:** Meta XR All-in-One SDK (camera rig, interaction rig, passthrough)
- **Target device:** Meta Quest 3S (single-player and client) / Windows PC (multiplayer server)
- **Networking:** PurrNet (LAN discovery + UDP, authoritative server)
- **Data:** JSON serialisation, SMTP e-mail transmission (single-player)
- **Analysis:** Python / Google Colab (questionnaire scoring and statistics)

## Repository structure

```
.
├── README.md
├── .gitignore
├── docs/
│   └── FedericaCirillo_tesiMagistrale.pdf   # full thesis (PDF)
├── Assets/                                  # Unity project assets and scripts
├── Packages/                                # Unity package manifest
└── ProjectSettings/                         # Unity project settings
```

Auto-generated Unity folders (`Library/`, `Temp/`, `Obj/`, `Build/`, `Logs/`,
`UserSettings/`) are intentionally excluded — they are rebuilt by Unity when the project is opened.

## Getting started

1. Clone the repository:
   ```bash
   git clone https://github.com/<your-username>/<repo-name>.git
   ```
2. Open the project with **Unity 6 LTS** through Unity Hub (let it import packages on first launch).
3. Install the **Meta XR All-in-One SDK** if it is not resolved automatically.
4. To deploy on device, switch the build target to **Android** and build the APK for the Meta Quest 3S.

> **Note on credentials.** The single-player e-mail export relies on SMTP with a dedicated Google
> app password. Make sure no real credentials are committed to the repository; keep them in a local,
> untracked configuration.

## Evaluation

The single-player system was evaluated with the **SUS**, **UEQ-S** and **NASA-TLX** questionnaires on
a group of healthy participants, and compared against an equivalent group that had tested the previous
HoloLens prototype. Results showed high perceived usability and user experience together with a low
cognitive and motor workload. Compared with HoloLens, the Meta Quest version achieved comparable
usability and user experience with a **significantly lower subjective workload**, suggesting that the
new platform preserves system quality while improving accessibility.

## Thesis

The complete thesis (in Italian) is available in [`docs/`](docs/). It covers the clinical background
on Parkinson's disease, serious games and extended reality, the design and implementation of both game
modes, the experimental protocol, and the discussion of results, with the developed scripts reported
in the appendix.

## Author & supervisors

- **Author:** Federica Cirillo — M.Sc. in Biomedical Engineering (LM-21)
- **Supervisors:** Prof.ssa Ersilia Vallefuoco, Prof. Alessandro Pepino
- **Institution:** Department of Electrical Engineering and Information Technology (DIETI),
  University of Naples Federico II
- **Academic year:** 2025–2026

## Acknowledgements

Developed in collaboration with the Neurology Department of the A.O.U. Federico II of Naples and with
WipLab Srl.
