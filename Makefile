init-tools:
	sh ./eng/init-tools.sh

gen-code:
	sh ./eng/gen-code.sh

test:
	sh ./eng/test.sh

update:
	sh ./eng/update-tools.sh

clean:
	rm -rf ./bin
	rm -rf ./tmp
	rm -rf ./pub

.PHONY: all test clean
