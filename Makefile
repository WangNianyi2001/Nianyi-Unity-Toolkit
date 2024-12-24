entry:
	@echo "No make command given.";

urls:
	for pj in ./*/package.json; do\
		cat "$$pj" | sed -e 's/file:\.\.\//git\+https:\/\/github.com\/WangNianyi2001\/Nianyi-Unity-Toolkit.git\?path=/g' > $$pj;\
	done;