function prepareDrop(_this, elem) {
    if (elem === null) return;

    elem.addEventListener('dragover', e => e.preventDefault());
    elem.addEventListener('drop', e => {
        e.preventDefault();

        const blobUrls = [];
        const files = e.dataTransfer.files;
        for (let i = 0; i < files.length; i++) {
            const file = files[i];
            const reader = new FileReader();
            reader.onload = f => {
                fetch(f.target.result)
                    .then(res => res.blob())
                    .then(blob => {
                        blobUrls.push(URL.createObjectURL(blob));
                        if (blobUrls.length === files.length)
                        {
                            _this.invokeMethodAsync("JSOnDropFiles", blobUrls);
                        }
                    });
            };

            reader.readAsDataURL(file);
        }
    });
}

function scrollToTop(elem) {
    if (elem === null) return;

    elem.scrollTo(0, 0);
}