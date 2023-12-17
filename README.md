# Lab2_Threads

It is good enough - I hope!
At least I gave threads a real match.

You choose amount of cars in the race and name them.
It is easier to follow the race with fewer cars - that's why I recommend a maximum of 5 cars.
Press Enter for race updates.

Unsolved issues:
- I didn't succeed using universal "seconds" for each car's racing. Neither with nor without help of localSeconds.Value.
- I am actually not sure if i cancelled all threads... 
  I wait for the racethreads to be joined and then cancel the other two so should be OK - or?
- Distance shouldn't be a car property but a ThreadLocal of the race, I  guess.
- When getting updates on the race, cars during their stops should have speed 0 m/h, 
  now it shows their current speed during the race (when not stopped)
- There must be a prettier way of rewriting a line with shorter text than filling it with spaces (e.g. $"* {car.Name} är i mål                                                                                       ")

Could have been added (if I didn't fight with seconds):
- Another "obstacle": hit a human -> withdraw from race => if one car left end the race(or continue but say sth)

Not sure if it is good enough :(
- Especially if you choose to race a shorter distance (e.g. 3km) the winner might be unexpected. 
  As far as I understand it, it happens because of the following:
	* I update distance every second which means 33,(3)m at the original speed of 120km/h or 33,06m at 119km/h.
	* I only check if a car passed the goal after a whole second.
  It is possible that while competing at a short distance the only problem
  one of the car gets causes it to slow down towards the end of the race. 
  That slower car can still cross the goal line first and win if this thread updated first 
  and the distance between competing cars was not yet big enough. :(
