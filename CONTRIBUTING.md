Contributing to Luthetus.Ide
============================

Pull requests (PRs)
---------------------

- Target the 'staging' branch
- Create a draft pull request.
- Avoid cosmetic changes to unrelated files in the same commit.
- I (Hunter Freeman), plan to merge 'staging' into 'main' on a weekly basis.

Summary:
PRs will be accepted into 'staging' throughout the week.
At the end of the week I (Hunter Freeman), will test 'staging'.
After which I will merge 'staging' into 'main'.

Side notes:
- I view accepting a PR as an extreme security risk. I will be meticulously looking for malicious code, perhaps hidden within a seemingly innocuous PR.
- I need to build up the test suite more. This way people can be more confident in their changes not breaking something prior to a maintainer looking at it.

Applications to test before accepting PR
----------------------------------------
-Luthetus.Ide.Photino.csproj
	-Linux
	-Windows
	-Mac
-Luthetus.Ide.Wasm.csproj
	-Chromium
	-Firefox
-Luthetus.Ide.ServerSide.csproj
	-Chromium
	-Firefox

Unit test before accepting PR
-----------------------------
-For each PR keep track of the pass/fail count after running every unit test on 'master' branch vs 'staging' branch.