import nltk
import spacy
  
# essential entity models downloads
#nltk.downloader.download('maxent_ne_chunker')
#nltk.downloader.download('words')
#nltk.downloader.download('treebank')
#nltk.downloader.download('maxent_treebank_pos_tagger')
#nltk.downloader.download('punkt')
#nltk.download('averaged_perceptron_tagger')

import locationtagger
  
# initializing sample text
sample_text = "India has very rich and vivid culture\
       widely spread from Kerala to Nagaland to Haryana to Maharashtra. " \
       "Delhi being capital with Mumbai financial capital.\
       Can be said better than some western cities such as " \
       " Munich, London etc. Pakistan and Bangladesh share its borders"
  
# extracting entities.
place_entity = locationtagger.find_locations(text = sample_text)
  
# getting all countries
print("The countries in text : ")
print(place_entity.countries)
  
# getting all states
print("The states in text : ")
print(place_entity.regions)
  
# getting all cities
print("The cities in text : ")
print(place_entity.cities)


import re

txt = "Worten Arrábida Shopping Praceta Henrique Moreira 244 - Afurada | Arrábida Shopping Loja 4.57/4.61 Piso 0 4400-346 Vila Nova de Gaia Todos os dias: 10:00 - 23:00 VER MAIS"
regexp = "[0-9]{1,3} .+, .+, [A-Z]{2} [0-9]{5}"
address = re.findall(regexp, txt)
print("address is " + str(address.__len__()))