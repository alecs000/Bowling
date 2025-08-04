import sys
from PIL import Image

try:
    # Getting values from arguments
    image_full_names = sys.argv[1:]

    # Cropping images
    for image_full_name in image_full_names:
        if (".png" in image_full_name):
            image = Image.open(image_full_name)
            image_box = image.getbbox()
            cropped = image.crop(image_box)
            cropped.save(image_full_name)

    print(1)
except:
    print(-1)
