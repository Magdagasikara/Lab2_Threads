# Lab2_Threads

it is good enough - I hope!

Unsolved issues:
- I didn't succeed using universal "seconds" for each car's racing. Neither with nor without help of localSeconds.Value.
- I am actually not sure if i cancelled all threads... 
  I wait for the racethreads to be joined and then cancel the other two so should be OK - or?
- Distance shouldn't be a car property but a ThreadLocal of the race.
- When getting info for cars during their stop should show speed 0 m/h during that time, 
  now it shows their current speed during the race (when not stopped)

Could have been added (if I didn't find with seconds):
- Another "obstacle": hit a human -> withdraw from race => if one car left end the race(or continue but say sth)