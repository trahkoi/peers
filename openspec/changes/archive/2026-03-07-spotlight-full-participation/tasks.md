## 1. Algorithm change

- [x] 1.1 Modify `PairingAlgorithm.GeneratePairings` to loop: after each greedy pass, collect unpaired dancers from the larger group, reset the smaller group, accumulate sub-round pairings as additional history, and repeat until all dancers have been paired
- [x] 1.2 Update existing unbalanced-group tests (`MoreLeaders_SomeLeadersSitOut`, `MoreFollowers_SomeFollowersSitOut`) to expect full participation instead of sit-outs

## 2. New test cases

- [x] 2.1 Add test: heavily unbalanced (6 leaders, 1 follower) produces 6 pairings with each leader appearing once
- [x] 2.2 Add test: followers rotate partners across sub-rounds (4 leaders, 2 followers, no history — each follower gets different leaders)
- [x] 2.3 Add test: determinism still holds with multi-sub-round output
- [x] 2.4 Add test: self-pair edge case with unbalanced groups (dancer in both lists) still works correctly
