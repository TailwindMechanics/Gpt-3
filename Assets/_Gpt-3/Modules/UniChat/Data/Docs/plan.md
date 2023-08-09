

### 1. UI Requirements
    - Button to advance conversation turn-by-turn
    - Display transcript of each turn's conversation and actions
    - Optionally display visual data received

### 2. Core Logic Requirements
    - Automatically capture visual scene data on every turn
    - Pass visual data to Brainwave each turn
    - Require a direction vector as input to move() each turn
    - Move Brainwave based on input vector with physics

### 3. Brainwave Conversation Requirements
    - Respond conversationally to visual data each turn
    - Decide on a direction vector to pass to move()
    - Verbally announce when proximity threshold reached