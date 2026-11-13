// Classes run in parallel; methods inside a class run sequentially. Tests that touch shared
// static state (Random.DefaultGenerator, thread culture) are marked [DoNotParallelize].
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
