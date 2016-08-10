from PIL import Image


def display(image_data, width, height):

    image_list = []
    for j in range(height):
        for i in range(width):
            pixelInBytes = [int(255 * x) for x in image_data[i][height - j - 1]]
            image_list.append(tuple(pixelInBytes))

    img = Image.new('RGB', (width, height))
    img.putdata(image_list)
    img.save('image.png')

