import requests
from gallery_dl.extractor.common import GalleryExtractor

session = requests.Session()
req = session.get("https://nhentai.net/g/597688/")
print(req.text)